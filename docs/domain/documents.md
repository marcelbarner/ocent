# Documents Domain

The Documents domain manages all files that the household stores in ocent: bank statements, tax
documents, contracts, invoices, warranties, and any other uploaded files. Every document has a
version history and one or more physical files per version.

---

## 1. Document

The top-level entity. A Document represents a logical document — e.g. "Sparkasse Statement January
2026" — independent of how many file versions exist.

**Table name:** `documents`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK → `households.id`, NOT NULL | |
| `created_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `name` | `text` | NOT NULL | User-given document title |
| `document_type` | `text` | NOT NULL | Free-text category label (e.g. "BankStatement", "Invoice", "TaxReturn") |
| `issue_date` | `date` | nullable | Date shown on the document |
| `issuer` | `text` | nullable | Issuing organisation (e.g. "Sparkasse Berlin") |
| `status` | `document_status` | NOT NULL, default `Uploaded` | |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `notes` | `text` | nullable | User notes |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `document_status`:** `Uploaded | Processing | Enriched | Archived`

| Status | Meaning |
|--------|---------|
| `Uploaded` | File received, no further processing done |
| `Processing` | OCR or AI pipeline is actively running |
| `Enriched` | Pipeline completed; fields populated from extracted text |
| `Archived` | No longer actively referenced; retained for record |

**Index:** `(household_id, issue_date DESC)` — household document list ordered by date.
**Index:** `(household_id, document_type)` — filter by type.

---

## 2. DocumentVersion

Each time a Document is updated with a new file (revised scan, corrected version), a new
DocumentVersion is created. The current version is the one with the highest `version_number`.

**Table name:** `document_versions`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `document_id` | `uuid` | FK → `documents.id`, NOT NULL | |
| `version_number` | `integer` | NOT NULL | Monotonically increasing per document; starts at 1 |
| `change_description` | `text` | nullable | Why this version was created |
| `created_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(document_id, version_number)`

The application derives the current version by querying `MAX(version_number)` for a given
`document_id`. No "is_current" flag is stored — that flag would require transactional maintenance
on every version insert.

---

## 3. DocumentFile

The physical file associated with a DocumentVersion. A version may have exactly one file in the
initial implementation. The schema supports multiple files per version for future multi-page or
multi-attachment scenarios.

**Table name:** `document_files`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `document_version_id` | `uuid` | FK → `document_versions.id`, NOT NULL | |
| `storage_path` | `text` | NOT NULL | Relative path within the `ocent-file-storage` volume |
| `original_filename` | `text` | NOT NULL | Filename as uploaded by the user |
| `mime_type` | `text` | NOT NULL | MIME type determined at upload |
| `file_size_bytes` | `bigint` | NOT NULL | |
| `checksum_sha256` | `char(64)` | NOT NULL | SHA-256 hex digest of file content |
| `ocr_text` | `text` | nullable | Full extracted text from OCR pipeline |
| `ocr_completed_at` | `timestamptz` | nullable | When OCR finished |
| `uploaded_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(checksum_sha256, household_id)` — prevents duplicate file uploads within a
household. The application checks this before writing a new DocumentFile and reuses the existing
file record if the checksum matches (content-addressable deduplication at the record level).

**storage_path format:**

```
documents/{year}/{month}/{uuid}.{ext}
```

Example: `documents/2026/01/3f9a2c1e-4b5d-4e6f-8a7b-9c0d1e2f3a4b.pdf`

The path is always relative to `/app/storage/` in the volume. The backend resolves the absolute
path at serving time. Paths are never exposed to the client — the API streams file content through
an authenticated endpoint.

---

## Entity Relationships

```
Document
  └── DocumentVersion (1..*)
        └── DocumentFile (1..* per version; initially 1)
```

Cross-domain connections:
- A Document can be linked to Transactions via `Link` table (`link_type = finances_document`)
- A Document can be linked to ContractVersions via `Link` table (`link_type = contract_document`)
- A Document can be linked to Assets via `Link` table (`link_type = asset_document`)
- A Document can be a member of one or more Containers via `ContainerMembership`
- A PensionStatusReport references a Document via `linked_document_id`
