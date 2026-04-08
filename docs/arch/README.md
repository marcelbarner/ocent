# ocent -- Docker-basierte Gesamtarchitektur

> Self-hosted personal operations hub fuer Finanzen, Dokumente, Storage und Vertraege.

## Architektur-Prinzipien

1. **Single-Node Self-Hosting** -- ocent laeuft auf einem einzelnen Server (Homelab, VPS, NAS). Kein Cluster-Orchestrator, kein Kubernetes. Docker Compose ist das Deployment-Tool.
2. **Privacy-First** -- Keine Cloud-Abhaengigkeiten. Keine Telemetrie. Authentifizierung ist lokal integriert, kein externer Identity Provider.
3. **Immutable Containers** -- Alle Container sind stateless. Persistenz liegt ausschliesslich in benannten Docker Volumes.
4. **Explizite Konfiguration** -- Jeder Port, jedes Volume, jedes Netzwerk ist deklariert. Keine impliziten Defaults.
5. **Aspire fuer Dev, Compose fuer Prod** -- .NET Aspire orchestriert die lokale Entwicklung. Docker Compose uebernimmt Staging und Produktion. Die beiden Welten ueberlappen nicht.
6. **Least Privilege** -- Container laufen als non-root User. Der Reverse Proxy ist der einzige oeffentlich exponierte Service.

## Service-Uebersicht

```
                        Internet / LAN
                             |
                     +-------+-------+
                     |    Traefik    |  :443 (HTTPS)
                     |  Reverse Proxy|  :80  (HTTP -> Redirect)
                     +---+-------+---+
                         |       |
              +----------+       +----------+
              |                              |
     +--------+--------+          +---------+---------+
     |   Frontend       |          |   Backend API     |
     |   (Nginx static) |          |   (ASP.NET Core)  |
     |   :80 intern     |          |   :8080 intern    |
     +------------------+          +--------+----------+
                                            |
                                   +--------+----------+
                                   |   PostgreSQL      |
                                   |   :5432 intern    |
                                   +-------------------+
```

## Services

| Service        | Image                          | Zweck                                           |
|----------------|--------------------------------|------------------------------------------------|
| `traefik`      | `traefik:v3`                   | Reverse Proxy, TLS-Terminierung, Routing        |
| `frontend`     | Custom (multi-stage Nginx)     | Angular SPA, statisch via Nginx ausgeliefert    |
| `backend`      | Custom (multi-stage .NET)      | ASP.NET Core Web API, REST/OpenAPI              |
| `postgres`     | `postgres:17-alpine`           | Relationale Datenbank fuer alle Domaenen        |

## Vier Domaenen

Alle vier Domaenen (Finance, Documents, Storage, Contracts) leben im selben Backend-Service. Kein Microservice-Split -- das waere Over-Engineering fuer einen Single-User-Hub. Die Domaenen-Trennung erfolgt auf Code-Ebene (Module/Namespaces), nicht auf Container-Ebene.

**Begruendung:** Ein Haushalt erzeugt keine Last, die separate Services rechtfertigt. Ein Monolith-API mit sauberer interner Modularisierung ist einfacher zu deployen, zu debuggen und zu sichern.

## Authentifizierung

Authentifizierung ist direkt in die Backend-API integriert -- kein separater Auth-Container. Die API implementiert:

- Lokale Benutzerregistrierung mit Passwort-Hash (Argon2id)
- JWT-basierte Session-Tokens (kurze Laufzeit, 15 Minuten)
- Refresh-Tokens (HttpOnly Cookie, 7 Tage)
- Optional: TOTP-basiertes 2FA

**Begruendung gegen separaten Auth-Service (Keycloak, Authentik):** Fuer einen Single-User-/Haushalts-Hub ist ein vollstaendiger OIDC-Stack Overhead. Keycloak allein braucht ~512MB RAM und eine eigene Datenbank. Die Auth-Logik im Backend-Monolith ist ausreichend, wartbar und ressourcenschonend.

## Verzeichnisstruktur der Architektur-Docs

```
docs/arch/
  README.md          <- Dieses Dokument
  containers.md      <- Container-Design, Images, Multi-Stage Builds
  networking.md      <- Netzwerk-Topologie, Traefik, TLS
  data.md            <- Datenpersistenz, Volumes, Backup
  deployment.md      <- Dev vs. Prod Setup, Docker Compose Referenz
```

## Naechste Schritte

1. Dockerfiles fuer Backend und Frontend erstellen (Multi-Stage Builds)
2. `docker-compose.prod.yml` schreiben
3. Traefik-Konfiguration mit Let's Encrypt aufsetzen
4. CI-Pipeline fuer Image-Builds definieren
5. Backup-Skript fuer Volumes implementieren
