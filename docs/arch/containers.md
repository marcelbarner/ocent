# Container-Design

## Container-Strategie

ocent besteht aus vier Containern. Jeder hat eine klar definierte Verantwortung.

### 1. Backend API (`ocent-backend`)

**Base Image:** `mcr.microsoft.com/dotnet/aspnet:10.0-alpine` (Runtime), `mcr.microsoft.com/dotnet/sdk:10.0-alpine` (Build)

**Build-Strategie:** Multi-Stage Build mit drei Stufen:

```dockerfile
# Stage 1: Restore (cached bei unveraenderten .csproj)
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS restore
WORKDIR /src
COPY Directory.Packages.props Directory.Build.props* ./
COPY src/backend/src/ocent.Backend.WebApi/ocent.Backend.WebApi.csproj ./backend/
RUN dotnet restore ./backend/ocent.Backend.WebApi.csproj

# Stage 2: Build + Publish
FROM restore AS publish
COPY src/backend/src/ocent.Backend.WebApi/ ./backend/
RUN dotnet publish ./backend/ocent.Backend.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
RUN addgroup -S ocent && adduser -S ocent -G ocent
WORKDIR /app
COPY --from=publish /app/publish .
USER ocent
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "ocent.Backend.WebApi.dll"]
```

**Konfiguration:**
- Port: `8080` (intern, nicht exponiert nach aussen)
- Umgebungsvariablen ueber `.env`-Datei oder Docker Secrets
- `ASPNETCORE_ENVIRONMENT=Production`
- Connection String via `ConnectionStrings__DefaultConnection` Environment Variable

**Health Check:** HTTP GET auf `/health` -- der Backend muss einen Health-Endpoint bereitstellen (ASP.NET `MapHealthChecks`).

### 2. Frontend (`ocent-frontend`)

**Base Image:** `node:22-alpine` (Build), `nginx:1.27-alpine` (Runtime)

**Build-Strategie:** Multi-Stage Build:

```dockerfile
# Stage 1: Build
FROM node:22-alpine AS build
WORKDIR /app
COPY src/frontend/package.json src/frontend/package-lock.json ./
RUN npm ci --ignore-scripts
COPY src/frontend/ .
RUN npm run build -- --configuration production

# Stage 2: Runtime
FROM nginx:1.27-alpine AS runtime
RUN rm /etc/nginx/conf.d/default.conf
COPY deploy/nginx/frontend.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
RUN addgroup -S ocent && adduser -S ocent -G ocent && \
    chown -R ocent:ocent /usr/share/nginx/html /var/cache/nginx /var/run
USER ocent
EXPOSE 80
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/ || exit 1
```

**Begruendung fuer Nginx statt Node-basiertem Serving:**
- Angular produziert statische Assets. Ein Node-Server waere Verschwendung.
- Nginx ist schneller, braucht weniger RAM (~5MB vs. ~50MB) und kann Caching-Header, Gzip und SPA-Fallback nativ.
- Die Vite-Dev-Server-Architektur ist irrelevant fuer Produktion.

**Nginx-Konfiguration (`frontend.conf`):**
```nginx
server {
    listen 80;
    root /usr/share/nginx/html;
    index index.html;

    # SPA Fallback
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Cache statische Assets aggressiv
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff2?)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # Keine Caching fuer index.html
    location = /index.html {
        add_header Cache-Control "no-cache, no-store, must-revalidate";
    }
}
```

### 3. PostgreSQL (`ocent-db`)

**Image:** `postgres:17-alpine`

**Begruendung PostgreSQL statt SQLite:**
- SQLite hat keine echte Concurrency -- bei gleichzeitigen Schreibzugriffen (z.B. Hintergrund-Jobs fuer Dokumenten-Processing) gibt es Locking-Probleme.
- PostgreSQL bietet Full-Text-Search (relevant fuer Dokumente), JSON-Columns (flexibel fuer Vertragsmetadaten) und echte Transaktionssicherheit.
- Der RAM-Overhead ist akzeptabel: PostgreSQL Alpine braucht ~30MB im Idle.
- Backup/Restore ist mit `pg_dump` / `pg_restore` battle-tested.

**Konfiguration:**
```yaml
environment:
    POSTGRES_DB: ocent
    POSTGRES_USER: ocent
    POSTGRES_PASSWORD_FILE: /run/secrets/db_password
healthcheck:
    test: ["CMD-SHELL", "pg_isready -U ocent -d ocent"]
    interval: 10s
    timeout: 5s
    retries: 5
    start_period: 30s
```

### 4. Traefik (`ocent-proxy`)

**Image:** `traefik:v3`

Details in [networking.md](./networking.md).

## Image-Tagging-Strategie

- Production-Images werden mit Git-SHA und semantischer Version getagged: `ocent-backend:1.2.3` und `ocent-backend:abc1234`
- `latest` wird nur fuer lokale Entwicklung verwendet, nie in Produktion referenziert
- Images werden lokal gebaut (`docker compose build`), kein Container Registry noetig fuer Self-Hosting

## Non-Root Execution

Alle Custom-Images erstellen einen dedizierten `ocent` User/Group. Kein Container laeuft als root. PostgreSQL und Traefik offizielle Images bringen eigene non-root-Konfigurationen mit.

## Ressourcen-Limits

Fuer Self-Hosting auf begrenzter Hardware werden Ressourcen-Limits gesetzt:

| Service    | Memory Limit | CPU Limit |
|------------|-------------|-----------|
| backend    | 256MB       | 0.5       |
| frontend   | 64MB        | 0.25      |
| postgres   | 256MB       | 0.5       |
| traefik    | 128MB       | 0.25      |
| **Gesamt** | **704MB**   | **1.5**   |

Diese Limits sind Ausgangswerte. Sie werden basierend auf tatsaechlicher Nutzung angepasst.
