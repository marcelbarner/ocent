# Datenpersistenz

## Entscheidung: PostgreSQL 17

**Gewaehlt: PostgreSQL 17 Alpine**

SQLite wurde bewusst verworfen. Begruendung:

| Kriterium              | SQLite                    | PostgreSQL                  |
|------------------------|---------------------------|-----------------------------|
| Concurrent Writes      | WAL-Modus, aber ein Writer| Volle Concurrency           |
| Full-Text Search       | FTS5 (limitiert)          | `tsvector` (maechtig)       |
| JSON-Daten             | json_extract (basic)      | `jsonb` (indexierbar)       |
| Backup im Betrieb      | Problematisch             | `pg_dump` waehrend Betrieb  |
| RAM-Verbrauch          | ~0 MB (in-process)        | ~30 MB idle                 |
| Skalierung spaeter     | Migration noetig          | Laeuft einfach weiter       |

Fuer ocent sind Concurrent Writes (Hintergrund-Jobs fuer Dokument-Indexierung), Full-Text Search (Dokumentensuche) und JSONB (flexible Vertragsmetadaten) entscheidende Faktoren.

## Volume-Strategie

Vier benannte Volumes decken alle persistenten Daten ab:

```yaml
volumes:
    ocent-db-data:
        driver: local
    ocent-file-storage:
        driver: local
    ocent-acme:
        driver: local
    ocent-config:
        driver: local
```

### Volume-Zuordnung

| Volume              | Container  | Mount-Pfad                  | Inhalt                          |
|---------------------|-----------|-----------------------------|---------------------------------|
| `ocent-db-data`     | postgres  | `/var/lib/postgresql/data`  | PostgreSQL Datenbankdateien     |
| `ocent-file-storage`| backend   | `/app/storage`              | Hochgeladene Dokumente/Dateien  |
| `ocent-acme`        | traefik   | `/acme`                     | Let's Encrypt Zertifikate       |
| `ocent-config`      | traefik   | `/etc/traefik`              | Traefik statische Konfiguration |

### Warum Named Volumes statt Bind Mounts

- Named Volumes werden von Docker verwaltet -- Berechtigungen sind konsistent
- Portabilitaet: `docker volume` Befehle funktionieren ueberall gleich
- Backup ueber `docker run --volumes-from` moeglich
- Bind Mounts erfordern manuelle Rechteverwaltung und sind host-abhaengig

**Ausnahme:** Die Traefik-Konfigurationsdatei (`traefik.yml`) wird als Bind Mount eingebunden, da sie Teil des Repository ist und versioniert werden soll:
```yaml
volumes:
    - ./deploy/traefik/traefik.yml:/etc/traefik/traefik.yml:ro
```

## Datei-Storage (Documents/Storage-Domaene)

Hochgeladene Dateien (Dokumente, Vertraege, Belege) werden im Filesystem gespeichert, nicht in der Datenbank.

**Struktur im Volume:**
```
/app/storage/
  documents/
    {year}/
      {month}/
        {uuid}.{ext}       # Originaldatei
        {uuid}.meta.json   # Metadaten (optional, falls nicht in DB)
  contracts/
    {uuid}.pdf
  temp/
    {upload-id}/           # Temporaere Upload-Chunks
```

**Begruendung Filesystem statt Blob-in-DB:**
- PostgreSQL TOAST/BYTEA skaliert schlecht bei grossen Dateien (>1MB)
- Filesystem-Backups sind inkrementell moeglich (rsync)
- Streaming grosser Dateien ist direkt vom Filesystem performanter
- Die Datenbank speichert nur Referenzen (Pfad, Hash, MIME-Type, Groesse)

## Datenbank-Schema-Strategie

- Migrations werden mit Entity Framework Core Migrations verwaltet
- Die Backend-API fuehrt Migrations automatisch beim Start aus (`Database.Migrate()`)
- Fuer Production kann ein separater Migration-Step vor dem Deployment geschaltet werden

## Backup-Strategie

### Datenbank-Backup

```bash
# Taeglich via Cronjob auf dem Host
docker exec ocent-db pg_dump -U ocent -d ocent -Fc > /backup/ocent-db-$(date +%Y%m%d).dump

# Restore
docker exec -i ocent-db pg_restore -U ocent -d ocent --clean < /backup/ocent-db-20260408.dump
```

**Aufbewahrung:**
- Taeglich: letzte 7 Backups
- Woechentlich: letzte 4 Backups
- Monatlich: letzte 12 Backups

### Datei-Backup

```bash
# Inkrementelles Backup des File-Storage Volumes
docker run --rm \
    -v ocent-file-storage:/source:ro \
    -v /backup/files:/target \
    alpine rsync -av /source/ /target/
```

### Vollstaendiges System-Backup

Ein einzelnes Backup-Skript (`deploy/backup.sh`) orchestriert:
1. PostgreSQL Dump
2. File-Storage rsync
3. ACME-Volume Kopie (Zertifikate)
4. Rotation alter Backups

### Disaster Recovery

Wiederherstellung aus Backup:
1. Docker Compose stoppen
2. Volumes neu erstellen
3. PostgreSQL Dump restoren
4. File-Storage Backup einspielen
5. Docker Compose starten -- Migrations laufen automatisch

**RTO (Recovery Time Objective):** < 30 Minuten
**RPO (Recovery Point Objective):** 24 Stunden (taeglich Backup)

## Secrets-Management

Datenbank-Passwoerter und andere Secrets werden nicht in Docker Compose Files oder Images gespeichert.

**Ansatz: Docker Secrets ueber Dateien**

```yaml
secrets:
    db_password:
        file: ./secrets/db_password.txt
    jwt_signing_key:
        file: ./secrets/jwt_signing_key.txt

services:
    postgres:
        secrets:
            - db_password
        environment:
            POSTGRES_PASSWORD_FILE: /run/secrets/db_password
    backend:
        secrets:
            - db_password
            - jwt_signing_key
```

Das `secrets/`-Verzeichnis ist in `.gitignore` eingetragen. Ein `deploy/init-secrets.sh` Skript generiert initiale Secrets bei Erstinstallation.

## Offene Fragen

1. **Verschluesselung at Rest:** Sollen hochgeladene Dokumente im Volume verschluesselt werden? Erhoehter Komplexitaet vs. Sicherheitsgewinn bei Self-Hosting.
2. **Datei-Deduplizierung:** Content-Addressable Storage (nach SHA256) wuerde Speicher sparen, erhoehte aber die Komplexitaet der Loeschlogik.
3. **Suchindex:** Reicht PostgreSQL Full-Text Search fuer die Dokumentensuche, oder wird spaeter ein dedizierter Suchindex (z.B. Meilisearch) benoetigt?
