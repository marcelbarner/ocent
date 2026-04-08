# Netzwerk-Topologie

## Entscheidung: Traefik als Reverse Proxy

**Gewaehlt: Traefik v3**

**Begruendung gegen Nginx (als Reverse Proxy):**
- Traefik hat native Docker-Integration -- neue Services werden automatisch erkannt via Labels, kein manuelles Config-Reload.
- Let's Encrypt ACME ist eingebaut -- kein Certbot-Sidecar oder Cronjob noetig.
- Dashboard fuer Debugging inklusive.
- Nginx als Reverse Proxy erfordert manuelles Template-Management und Reload-Mechanismen. Das ist fuer einen Self-Hosted-Hub unnoetig komplex.

**Hinweis:** Nginx wird trotzdem verwendet -- aber nur intern als Static-File-Server fuer das Angular-Frontend. Traefik und der interne Nginx haben unterschiedliche Rollen.

## Netzwerk-Segmentierung

Zwei Docker Bridge Networks trennen Frontend-Traffic von Datenbank-Traffic:

```
                    Internet
                       |
              +--------+--------+
              |    traefik      |
              +---+--------+---+
                  |        |
        +---------+        +---------+
        |  ocent-frontend-net        |
        |                            |
   +----+----+              +--------+--------+
   | frontend |              |    backend      |
   +----------+              +---+--------+---+
                                 |        |
                        +--------+        |
                        |  ocent-backend-net
                        |                 |
                   +----+------+          |
                   | postgres  |          |
                   +-----------+     (kein Zugang
                                      zum Frontend)
```

### Network-Definitionen

```yaml
networks:
    ocent-frontend-net:
        driver: bridge
        internal: false
    ocent-backend-net:
        driver: bridge
        internal: true
```

### Service-Netzwerk-Zuordnung

| Service    | ocent-frontend-net | ocent-backend-net |
|------------|-------------------|-------------------|
| traefik    | ja                | nein              |
| frontend   | ja                | nein              |
| backend    | ja                | ja                |
| postgres   | nein              | ja                |

**Prinzip:** PostgreSQL ist nur vom Backend erreichbar. Das Frontend hat keinen direkten Datenbankzugang. Traefik sieht nur die HTTP-Services.

## Traefik-Konfiguration

### Statische Konfiguration (`traefik.yml`)

```yaml
api:
    dashboard: true
    insecure: false

entryPoints:
    web:
        address: ":80"
        http:
            redirections:
                entryPoint:
                    to: websecure
                    scheme: https
    websecure:
        address: ":443"

certificatesResolvers:
    letsencrypt:
        acme:
            email: "${ACME_EMAIL}"
            storage: /acme/acme.json
            httpChallenge:
                entryPoint: web

providers:
    docker:
        exposedByDefault: false
        network: ocent-frontend-net

log:
    level: WARN
```

### Service-Labels (Docker Compose)

**Frontend:**
```yaml
labels:
    - "traefik.enable=true"
    - "traefik.http.routers.frontend.rule=Host(`${OCENT_DOMAIN}`)"
    - "traefik.http.routers.frontend.entrypoints=websecure"
    - "traefik.http.routers.frontend.tls.certresolver=letsencrypt"
    - "traefik.http.services.frontend.loadbalancer.server.port=80"
```

**Backend API:**
```yaml
labels:
    - "traefik.enable=true"
    - "traefik.http.routers.api.rule=Host(`${OCENT_DOMAIN}`) && PathPrefix(`/api`)"
    - "traefik.http.routers.api.entrypoints=websecure"
    - "traefik.http.routers.api.tls.certresolver=letsencrypt"
    - "traefik.http.services.api.loadbalancer.server.port=8080"
```

## TLS-Strategie

### Oeffentliches Deployment (VPS mit Domain)

- Let's Encrypt via Traefik ACME HTTP-01 Challenge
- Automatische Zertifikatserneuerung
- ACME-Storage in benanntem Volume (`ocent-acme`)
- HTTP wird zu HTTPS redirected

### Lokales Netzwerk (Homelab ohne oeffentliche Domain)

Zwei Optionen:

**Option A -- Self-Signed Zertifikate (empfohlen fuer Homelab):**
```yaml
# traefik.yml Ergaenzung
tls:
    certificates:
        - certFile: /certs/ocent.crt
          keyFile: /certs/ocent.key
```
Zertifikate werden einmalig mit `mkcert` generiert und via Volume gemounted.

**Option B -- Kein TLS (nur im isolierten LAN):**
Die HTTPS-Redirect-Regel wird entfernt. Traefik lauscht nur auf Port 80. Nur akzeptabel, wenn der Server ausschliesslich im privaten Netz erreichbar ist.

## Routing-Tabelle

| Pfad              | Ziel-Service | Port  | Beschreibung              |
|-------------------|-------------|-------|---------------------------|
| `/`               | frontend    | 80    | Angular SPA               |
| `/api/*`          | backend     | 8080  | REST API                  |
| `/api/health`     | backend     | 8080  | Health Check Endpoint     |

## Security-Header

Traefik setzt via Middleware folgende Security-Header:

```yaml
labels:
    - "traefik.http.middlewares.security.headers.browserXssFilter=true"
    - "traefik.http.middlewares.security.headers.contentTypeNosniff=true"
    - "traefik.http.middlewares.security.headers.frameDeny=true"
    - "traefik.http.middlewares.security.headers.stsIncludeSubdomains=true"
    - "traefik.http.middlewares.security.headers.stsSeconds=31536000"
```

## DNS-Anforderungen

- Fuer oeffentliches Deployment: Ein A-Record zeigt auf die Server-IP
- Fuer Homelab: Lokaler DNS-Eintrag (Pi-hole, Unbound, hosts-Datei) oder Zugriff via IP
- Nur eine Domain noetig -- Frontend und API laufen unter derselben Domain mit Path-basiertem Routing
