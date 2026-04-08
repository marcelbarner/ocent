# Shared Entities

All entities in this document appear across two or more domains or form the structural backbone of
the system. They are defined once here and referenced by every domain specification.

---

## 1. Household

The root ownership boundary. One deployment contains exactly one household.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK, default `gen_random_uuid()` | Stable identifier |
| `name` | `text` | NOT NULL | Display name, e.g. "Barner Family" |
| `created_at` | `timestamptz` | NOT NULL, default `now()` | Creation timestamp |
| `updated_at` | `timestamptz` | NOT NULL | Last modification timestamp |

**Table name:** `households`

No soft-delete. The household row is permanent for the lifetime of the installation.

---

## 2. User

A person with access to the household. The system supports one Owner and zero or more Members.
Authentication credentials are stored directly on the User record.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK, default `gen_random_uuid()` | Stable identifier |
| `household_id` | `uuid` | FK → `households.id`, NOT NULL | Parent household |
| `email` | `text` | UNIQUE, NOT NULL | Login identifier |
| `display_name` | `text` | NOT NULL | Shown in UI and provenance records |
| `password_hash` | `text` | NOT NULL | Argon2id hash |
| `totp_secret` | `text` | nullable | Base32-encoded TOTP seed; null means TOTP disabled |
| `role` | `user_role` | NOT NULL | Enum: `Owner` or `Member` |
| `visibility_default` | `visibility` | NOT NULL, default `Shared` | Default visibility for records created by this user |
| `is_active` | `boolean` | NOT NULL, default `true` | Soft-disable without deletion |
| `created_at` | `timestamptz` | NOT NULL, default `now()` | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Table name:** `users`

**Enum `user_role`:** `Owner | Member`

**Enum `visibility`:** `Shared | Private`

- `Owner`: exactly one per household; full access to all records regardless of visibility.
- `Member`: access limited by `visibility` flags on individual records.
- `totp_secret` is stored encrypted at the application layer using the deployment's secret key.
- `visibility_default` is applied at record creation time; it is not a live filter.

---

## 3. Ownership Model

Every domain entity that can be owned carries three ownership columns. These are not a separate
table — they are embedded columns on each entity table.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `household_id` | `uuid` | FK → `households.id`, NOT NULL | Owning household |
| `created_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | User who created the record |
| `visibility` | `visibility` | NOT NULL | `Shared` or `Private` |
| `write_restricted` | `boolean` | NOT NULL, default `false` | When true, only the `created_by_user_id` user can modify |

**Rules:**
- `Private` records are visible only to `created_by_user_id` and the household `Owner`.
- `write_restricted = true` means Members other than the creator cannot modify the record. The
  household Owner can always modify any record.
- `household_id` is always the household of the `created_by_user_id` user. There is no
  cross-household sharing.

---

## 4. Container

A named cross-domain grouping. A Container can hold entities from Finance, Documents, Storage, and
Contracts simultaneously. Examples: "BMW X5", "Apartment Berlin Hauptstraße", "Tax Year 2025".

**Table name:** `containers`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | Stable identifier |
| `household_id` | `uuid` | FK → `households.id`, NOT NULL | |
| `created_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `name` | `text` | NOT NULL | Display name |
| `description` | `text` | nullable | Optional free-text description |
| `kind` | `container_kind` | NOT NULL | Enum (see below) |
| `status` | `container_status` | NOT NULL, default `Active` | Enum (see below) |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | Cross-domain stage enum |
| `metadata` | `jsonb` | NOT NULL, default `'{}'` | Kind-specific metadata |
| `created_at` | `timestamptz` | NOT NULL, default `now()` | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `container_kind`:** `General | TaxCase | Event | Case`

| Kind | Intended use |
|------|-------------|
| `General` | Catch-all grouping (vehicle, property, appliance) |
| `TaxCase` | Tax year bundle — groups documents, transactions, contracts for one tax year |
| `Event` | Time-bounded event (move, renovation, purchase) |
| `Case` | Ongoing administrative case (insurance claim, legal matter) |

**Enum `container_status`:** `Active | Closed | Archived`

**`metadata` jsonb shape by kind:**

```json
// TaxCase
{ "tax_year": 2025, "filing_deadline": "2026-07-31", "filed_at": null }

// Event
{ "start_date": "2025-03-01", "end_date": "2025-04-15" }

// General / Case
{}
```

### ContainerMembership

Join table linking any entity to a Container. The `entity_type` discriminator string matches the
PostgreSQL table name of the referenced entity.

**Table name:** `container_memberships`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `container_id` | `uuid` | FK → `containers.id`, NOT NULL | |
| `entity_type` | `text` | NOT NULL | Table name string, e.g. `transactions`, `documents` |
| `entity_id` | `uuid` | NOT NULL | ID of the referenced entity |
| `added_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | Who added this membership |
| `added_at` | `timestamptz` | NOT NULL, default `now()` | |

**Unique constraint:** `(container_id, entity_type, entity_id)`

**Index:** `(entity_type, entity_id)` for reverse lookup ("which containers does this entity
belong to?").

There is no foreign key on `entity_id` because PostgreSQL does not support polymorphic foreign keys.
Referential integrity is enforced at the application layer. Orphan membership rows are cleaned up
by a periodic maintenance job.

---

## 5. Tag and EntityTag

Tags are free-form labels scoped to a household. A tag can be applied to any entity.

**Table name:** `tags`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK → `households.id`, NOT NULL | |
| `name` | `text` | NOT NULL | Normalized lowercase, e.g. `tax-2025` |
| `created_at` | `timestamptz` | NOT NULL, default `now()` | |

**Unique constraint:** `(household_id, name)`

**Table name:** `entity_tags`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `tag_id` | `uuid` | FK → `tags.id`, NOT NULL | |
| `entity_type` | `text` | NOT NULL | Table name discriminator |
| `entity_id` | `uuid` | NOT NULL | Referenced entity ID |
| `tagged_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | |
| `tagged_at` | `timestamptz` | NOT NULL, default `now()` | |

**Unique constraint:** `(tag_id, entity_type, entity_id)`

**Index:** `(entity_type, entity_id)` for efficient "all tags for entity X" queries.

Tag names are stored in normalized form: lowercased, spaces replaced with hyphens. The UI displays
them with a `#` prefix which is not stored.

---

## 6. Link

A universal directed edge between any two entities. Replaces ad-hoc cross-domain foreign keys
with a typed, queryable relationship store.

**Table name:** `links`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK → `households.id`, NOT NULL | |
| `source_entity_type` | `text` | NOT NULL | Table name of source entity |
| `source_entity_id` | `uuid` | NOT NULL | ID of source entity |
| `target_entity_type` | `text` | NOT NULL | Table name of target entity |
| `target_entity_id` | `uuid` | NOT NULL | ID of target entity |
| `link_type` | `text` | NOT NULL | Semantic type (see type registry below) |
| `direction` | `link_direction` | NOT NULL | Enum: `Directed | Bidirectional` |
| `provenance_source` | `source_type` | NOT NULL, default `ManualEntry` | How this link was created |
| `created_by_user_id` | `uuid` | FK → `users.id`, NOT NULL | |
| `created_at` | `timestamptz` | NOT NULL, default `now()` | |
| `metadata` | `jsonb` | NOT NULL, default `'{}'` | Link-type-specific data |

**Enum `link_direction`:** `Directed | Bidirectional`

**Unique constraint:** `(source_entity_type, source_entity_id, target_entity_type, target_entity_id, link_type)`

**Indexes:**
- `(source_entity_type, source_entity_id)` — outbound links from an entity
- `(target_entity_type, target_entity_id)` — inbound links to an entity

### Link Type Registry

| `link_type` | Meaning | Typical source → target |
|-------------|---------|------------------------|
| `finances_document` | Transaction or account backed by a document | `transactions` → `documents` |
| `contract_document` | Contract version supported by a document | `contract_versions` → `documents` |
| `contract_transaction` | Payment linked to a contract version | `transactions` → `contract_versions` |
| `asset_transaction` | Purchase/sale transaction for an asset | `assets` → `transactions` |
| `asset_document` | Ownership document for an asset | `assets` → `documents` |
| `liability_transaction` | Repayment transaction for a liability | `liabilities` → `transactions` |
| `receivable_transaction` | Received payment for a receivable | `receivables` → `transactions` |
| `product_document` | Manual, receipt, or warranty for a product | `products` → `documents` |
| `recurring_contract` | RecurringPattern derived from a contract | `recurring_patterns` → `contract_versions` |

This registry is append-only. Removing a link type requires a migration.

---

## 7. Source

Enum describing the import channel that produced a record or field value. Used on `Link.provenance_source` and within `FieldProvenance`.

**Enum name:** `source_type`

| Value | Description |
|-------|-------------|
| `ManualEntry` | User typed the value directly in the UI |
| `CsvImport` | Bulk CSV file import (e.g. bank export) |
| `BulkUpload` | Multi-file drag-and-drop upload |
| `OCR` | Optical character recognition from a document file |
| `AIAgent` | AI pipeline proposed or derived the value |

---

## 8. FieldProvenance

Not a table — a `jsonb` column (`field_provenance`) present on every enrichable entity. The structure records, for each field that was derived by a non-manual source, how it was produced and whether a human has confirmed it.

**Sparse by design.** If a field has no entry in `field_provenance`, the value was manually entered. An empty `{}` object is the baseline state.

### JSON Structure

```json
{
  "amount": {
    "source": "CsvImport",
    "confidence": null,
    "reviewed": true,
    "reviewed_by_user_id": "550e8400-e29b-41d4-a716-446655440000",
    "reviewed_at": "2026-01-15T10:23:00Z"
  },
  "description": {
    "source": "OCR",
    "confidence": 0.94,
    "reviewed": false,
    "reviewed_by_user_id": null,
    "reviewed_at": null
  },
  "category_id": {
    "source": "AIAgent",
    "confidence": 0.81,
    "reviewed": false,
    "reviewed_by_user_id": null,
    "reviewed_at": null
  }
}
```

### Field Schema

| Key | Type | Description |
|-----|------|-------------|
| `source` | `source_type` string | Which system produced this field value |
| `confidence` | `float | null` | 0.0–1.0; only set for OCR and AIAgent sources |
| `reviewed` | `boolean` | Whether a human has confirmed this value |
| `reviewed_by_user_id` | `uuid | null` | Which user confirmed it |
| `reviewed_at` | `ISO 8601 string | null` | When the review happened |

When a user manually edits a field that had a non-manual provenance entry, the entry is removed
from the jsonb (the field becomes implicitly manual). The original auto-derived value is not
preserved in this structure — audit history belongs in a separate audit log (future feature).

---

## 9. LifecycleStage Enum

Cross-domain enum for coarse lifecycle state. Used on Containers and as a summary field on domain
entities for dashboard filtering. Does not replace domain-specific status enums.

**Enum name:** `lifecycle_stage`

| Value | Meaning |
|-------|---------|
| `Raw` | Record exists but has not been processed or verified. Data may be incomplete. |
| `Enriched` | Record has been augmented by import, OCR, or AI pipeline. Awaits human review. |
| `Verified` | A human has confirmed the data is correct and complete. |

All domain entities that carry `lifecycle_stage` default to `Raw` at creation. Transitions are
one-directional in normal operation: `Raw → Enriched → Verified`. Regression to a lower stage is
allowed by the system when re-processing occurs but must be explicit.
