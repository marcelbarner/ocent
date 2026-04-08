# Deployment

## Zwei Welten: Aspire (Dev) vs. Docker Compose (Prod)

```
+---------------------------+        +---------------------------+
|     Entwicklung           |        |     Produktion            |
|                           |        |                           |
|  .NET Aspire AppHost      |        |  Docker Compose           |
|  - dotnet run             |        |  - docker compose up      |
|  - Hot Reload             |        |  - Pre-built Images       |
|  - Aspire Dashboard       |        |  - Traefik Proxy          |
|  - Lokale Ports           |        |  - TLS Terminierung       |
|  - Keine Container noetig |        |  - Named Volumes          |
|  - DB: In-Memory/SQLite   |        |  - DB: PostgreSQL         |
+---------------------------+        +---------------------------+
```

### Klare Trennung

- Aspire orchestriert **nur** die lokale Entwicklung. Es startet Backend-Prozesse nativ, kein Docker.
- Docker Compose orchestriert **nur** Staging und Produktion. Es kennt Aspire nicht.
- Die beiden Systeme teilen sich **kein** Konfigurationsformat. Die Verbindung ist das gleiche Backend-Image.

### Warum kein Aspire-zu-Compose-Export

.NET Aspire kann theoretisch Docker Compose Manifeste generieren (`aspire publish`). Das wird bewusst nicht genutzt:

1. Generiertes Compose ist verbose und schwer wartbar
2. Traefik-Labels und Netzwerk-Segmentierung lassen sich nicht sinnvoll aus Aspire ableiten
3. Handgeschriebenes Compose ist expliziter und besser dokumentierbar
4. Die Produktions-Konfiguration hat andere Anforderungen (Secrets, Volumes, Health Checks) als Dev

## Verzeichnisstruktur fuer Deployment

```
deploy/
  docker-compose.yml           # Produktion
  docker-compose.override.yml  # Lokales Testen der Container (ohne Aspire)
  .env.example                 # Template fuer Umgebungsvariablen
  traefik/
    traefik.yml                # Statische Traefik-Konfiguration
  nginx/
    frontend.conf              # Nginx Config fuer Frontend-Container
  backup.sh                    # Backup-Skript
  init-secrets.sh              # Erstellt initiale Secrets
  secrets/                     # .gitignored -- nur auf dem Server
    db_password.txt
    jwt_signing_key.txt
```

## Docker Compose: Produktion

```yaml
# deploy/docker-compose.yml
name: ocent

services:
    traefik:
        image: traefik:v3
        restart: unless-stopped
        ports:
            - "80:80"
            - "443:443"
        volumes:
            - /var/run/docker.sock:/var/run/docker.sock:ro
            - ./traefik/traefik.yml:/etc/traefik/traefik.yml:ro
            - ocent-acme:/acme
        networks:
            - ocent-frontend-net
        deploy:
            resources:
                limits:
                    memory: 128M
                    cpus: "0.25"
        healthcheck:
            test: ["CMD", "traefik", "healthcheck", "--ping"]
            interval: 30s
            timeout: 5s
            retries: 3

    frontend:
        image: ocent-frontend:${OCENT_VERSION:-latest}
        build:
            context: ../
            dockerfile: src/frontend/Dockerfile
        restart: unless-stopped
        networks:
            - ocent-frontend-net
        labels:
            - "traefik.enable=true"
            - "traefik.http.routers.frontend.rule=Host(`${OCENT_DOMAIN}`)"
            - "traefik.http.routers.frontend.entrypoints=websecure"
            - "traefik.http.routers.frontend.tls.certresolver=letsencrypt"
            - "traefik.http.services.frontend.loadbalancer.server.port=80"
        deploy:
            resources:
                limits:
                    memory: 64M
                    cpus: "0.25"
        healthcheck:
            test: ["CMD", "wget", "--no-verbose", "--tries=1", "--spider", "http://localhost:80/"]
            interval: 30s
            timeout: 5s
            retries: 3

    backend:
        image: ocent-backend:${OCENT_VERSION:-latest}
        build:
            context: ../
            dockerfile: src/backend/src/ocent.Backend.WebApi/Dockerfile
        restart: unless-stopped
        environment:
            - ASPNETCORE_ENVIRONMENT=Production
            - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=ocent;Username=ocent;Password_File=/run/secrets/db_password
        secrets:
            - db_password
            - jwt_signing_key
        volumes:
            - ocent-file-storage:/app/storage
        networks:
            - ocent-frontend-net
            - ocent-backend-net
        labels:
            - "traefik.enable=true"
            - "traefik.http.routers.api.rule=Host(`${OCENT_DOMAIN}`) && PathPrefix(`/api`)"
            - "traefik.http.routers.api.entrypoints=websecure"
            - "traefik.http.routers.api.tls.certresolver=letsencrypt"
            - "traefik.http.services.api.loadbalancer.server.port=8080"
            - "traefik.docker.network=ocent-frontend-net"
        depends_on:
            postgres:
                condition: service_healthy
        deploy:
            resources:
                limits:
                    memory: 256M
                    cpus: "0.5"
        healthcheck:
            test: ["CMD", "wget", "--no-verbose", "--tries=1", "--spider", "http://localhost:8080/health"]
            interval: 30s
            timeout: 5s
            start_period: 15s
            retries: 3

    postgres:
        image: postgres:17-alpine
        restart: unless-stopped
        environment:
            POSTGRES_DB: ocent
            POSTGRES_USER: ocent
            POSTGRES_PASSWORD_FILE: /run/secrets/db_password
        secrets:
            - db_password
        volumes:
            - ocent-db-data:/var/lib/postgresql/data
        networks:
            - ocent-backend-net
        deploy:
            resources:
                limits:
                    memory: 256M
                    cpus: "0.5"
        healthcheck:
            test: ["CMD-SHELL", "pg_isready -U ocent -d ocent"]
            interval: 10s
            timeout: 5s
            start_period: 30s
            retries: 5

networks:
    ocent-frontend-net:
        driver: bridge
    ocent-backend-net:
        driver: bridge
        internal: true

volumes:
    ocent-db-data:
    ocent-file-storage:
    ocent-acme:

secrets:
    db_password:
        file: ./secrets/db_password.txt
    jwt_signing_key:
        file: ./secrets/jwt_signing_key.txt
```

## Docker Compose: Lokaler Container-Test

```yaml
# deploy/docker-compose.override.yml
# Ueberlagert docker-compose.yml fuer lokales Testen ohne TLS

services:
    traefik:
        ports:
            - "8080:8080"   # Dashboard
        command:
            - "--api.insecure=true"
            - "--entrypoints.web.address=:80"
            - "--providers.docker=true"
            - "--providers.docker.exposedbydefault=false"

    frontend:
        labels:
            - "traefik.http.routers.frontend.entrypoints=web"
            - "traefik.http.routers.frontend.tls=false"

    backend:
        labels:
            - "traefik.http.routers.api.entrypoints=web"
            - "traefik.http.routers.api.tls=false"
        environment:
            - ASPNETCORE_ENVIRONMENT=Development
```

## Umgebungsvariablen

```bash
# deploy/.env.example
OCENT_DOMAIN=ocent.example.com
OCENT_VERSION=latest
ACME_EMAIL=admin@example.com
```

## Deployment-Ablauf

### Erstinstallation

```bash
cd deploy

# 1. Secrets generieren
./init-secrets.sh

# 2. .env konfigurieren
cp .env.example .env
# OCENT_DOMAIN und ACME_EMAIL anpassen

# 3. Images bauen
docker compose build

# 4. Starten
docker compose up -d

# 5. Logs pruefen
docker compose logs -f
```

### Update

```bash
cd deploy

# 1. Neuen Code pullen
git pull

# 2. Images neu bauen
docker compose build

# 3. Rolling Restart
docker compose up -d --remove-orphans

# 4. Health pruefen
docker compose ps
```

### Rollback

```bash
# Zum vorherigen Image-Tag zurueck
OCENT_VERSION=1.2.2 docker compose up -d
```

## Aspire-Integration: Entwicklungsumgebung

Fuer die taegliche Entwicklung wird Aspire genutzt, nicht Docker:

```bash
# Backend + Frontend starten via Aspire
dotnet run --project src/DevEnvironment/src/ocent.DevEnvironment.AppHost
```

Aspire startet:
- Backend WebAPI als nativen .NET-Prozess (mit Hot Reload)
- Optional: PostgreSQL als Container (via Aspire Container-Integration)
- Aspire Dashboard fuer Traces, Logs, Metriken

**Datenbank in Entwicklung:**
- Aspire kann einen PostgreSQL-Container starten (konsistent mit Produktion)
- Alternativ: In-Memory-DB oder SQLite fuer schnelle Unit Tests
- Acceptance Tests (TUnit.Aspire) laufen gegen den echten Aspire-Stack

### Aspire AppHost Erweiterung (Empfehlung)

Der aktuelle AppHost registriert nur das Backend. Fuer vollstaendige Dev-Orchestrierung:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("ocentdb");

var backend = builder.AddProject<Projects.ocent_Backend_WebApi>("webapi")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
```

Das Frontend wird separat via `npm start` gestartet (Vite Dev Server mit Proxy auf Backend).

## CI-Pipeline (Konzept)

```
Push to main
    |
    +-> Lint + Format Check
    +-> .NET Tests (TUnit)
    +-> Frontend Tests (Vitest)
    +-> Mutation Tests (Stryker)
    |
    v
Quality Gate passed?
    |
    +-> Docker Build (Backend + Frontend)
    +-> Image-Tag mit Git-SHA
    +-> Optional: Push to Registry
```

Fuer reines Self-Hosting ohne Registry werden Images direkt auf dem Zielserver gebaut:

```bash
# Auf dem Server
git pull && docker compose build && docker compose up -d
```

## Monitoring (Ausblick)

Fuer spaetere Erweiterung:
- Traefik liefert Metriken nativ (Prometheus-kompatibel)
- ASP.NET Core hat OpenTelemetry-Integration (Aspire nutzt das bereits in Dev)
- Ein leichtgewichtiger Stack waere: Prometheus + Grafana als optionale Container
- Fuer den Start reicht `docker compose logs` und der Traefik-Dashboard
