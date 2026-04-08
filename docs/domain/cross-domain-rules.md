# Cross-Domain Rules

This document defines rules that apply across domain boundaries: how entities are linked, who owns
what, how provenance is propagated, and what decisions remain open.

---

## 1. Link Rules

The `Link` table (defined in `shared-entities.md`) is the only mechanism for expressing
relationships between entities in different domains that are not:
- `ContainerMembership` (which entity belongs to which Container)
- Direct foreign keys within a domain (e.g. `Transaction.category_id`)

### Permitted Link Types

| link_type | Valid source types | Valid target types | direction |
|-----------|-------------------|-------------------|-----------|
| `finances_document` | `transactions`, `finance_accounts` | `documents` | Directed |
| `contract_document` | `contract_versions` | `documents` | Directed |
| `contract_transaction` | `transactions` | `contract_versions` | Directed |
| `asset_transaction` | `assets` | `transactions` | Directed |
| `asset_document` | `assets` | `documents` | Directed |
| `liability_transaction` | `liabilities` | `transactions` | Directed |
| `receivable_transaction` | `receivables` | `transactions` | Directed |
| `product_document` | `products` | `documents` | Directed |
| `recurring_contract` | `recurring_patterns` | `contract_versions` | Directed |

### Link Creation Rules

1. Both the source and target entity must belong to the same `household_id`. The application
   enforces this before inserting a Link row.
2. Creating a link does not change the `status` or `lifecycle_stage` of either entity. Only
   explicit pipeline steps or user actions change lifecycle state.
3. A Link may be created by the user manually or by an AI/import pipeline. The `provenance_source`
   column records which. AI-created links have `provenance_source = AIAgent` and require user
   review before they are treated as confirmed.
4. Deleting a source or target entity cascades to delete all Link rows referencing that entity.
   This cascade is implemented at the application layer, not via database CASCADE, because
   PostgreSQL cannot express polymorphic foreign key cascades.
5. Duplicate links (same source, target, and link_type) are prevented by the unique index on the
   `links` table.

### What Is Not a Link

- A `Transaction.category_id` is a direct foreign key within Finance, not a Link.
- A `ProviderContract.contract_subject_id` is a direct FK within Contracts, not a Link.
- Container memberships are in `ContainerMembership`, not Links.
- A `PensionStatusReport.linked_document_id` is a direct FK, not a Link (same domain boundary).

---

## 2. Ownership Rules

### Inheritance

Every entity that carries ownership columns (`household_id`, `created_by_user_id`, `visibility`,
`write_restricted`) is called an "owned entity." Child records of an owned entity do not duplicate
ownership columns — they inherit the household and visibility of their parent.

Example:
- `Transaction` is owned (has ownership columns).
- `TransactionSplit` is not independently owned — it inherits from its parent Transaction.
- `DocumentVersion` inherits from `Document`.
- `DocumentFile` inherits from `Document` via `DocumentVersion`.
- `ConditionPhase` inherits from `ProviderContract` via `ContractVersion`.

### Visibility Enforcement

The application layer enforces visibility on every read query:

- `Shared` records: visible to all active users in the household.
- `Private` records: visible only to `created_by_user_id` and users with `role = Owner`.
- The `household_id` filter is always applied first. No entity from another household is ever
  accessible regardless of other conditions.
- Visibility is enforced at the repository/query layer, not via PostgreSQL row-level security.
  There is no multitenancy isolation requirement beyond the single-household constraint.

### Write Restriction

- `write_restricted = false` (default): any authenticated household member can modify the record.
- `write_restricted = true`: only `created_by_user_id` and `role = Owner` users can modify.
- The Owner can override `write_restricted` on any record. This is by design for administrative
  access.
- Write restriction does not affect read access — it is purely a write control.

### Deletion

No domain entity supports hard deletion by regular users in the initial implementation. All
deletion is soft: entities gain a status value that marks them as archived or closed. Hard deletion
is an administrative operation reserved for the household Owner and only accessible via an explicit
admin UI path.

---

## 3. Provenance Rules

### Sparse field_provenance

The `field_provenance jsonb` column is present on every enrichable entity. The rule is:

- **No entry = manual.** If a field has no key in `field_provenance`, the value was typed by a
  user. There is no `source: ManualEntry` entry stored for manual fields.
- **Entry exists = non-manual.** An entry is present only when a field was populated by an import,
  OCR extraction, or AI pipeline.
- **Reviewed entries.** Once a user confirms an auto-derived field value, `reviewed = true`,
  `reviewed_by_user_id`, and `reviewed_at` are set. The entry remains — the field is no longer
  unconfirmed but the provenance history is preserved.
- **User edits over auto-derived value.** If the user manually edits a field that had a provenance
  entry, the entry is removed. The field becomes implicitly manual. The previous auto-derived value
  is not stored in `field_provenance`.

### LifecycleStage Transitions

`lifecycle_stage` on an entity changes as follows:

| From | To | Trigger |
|------|----|---------| 
| `Raw` | `Enriched` | An import, OCR, or AI pipeline has written at least one field with provenance |
| `Enriched` | `Verified` | A user explicitly confirms the record is complete and correct |
| `Verified` | `Enriched` | A background pipeline re-processes the record and writes new provenance |
| `Enriched` | `Raw` | All provenance entries are removed (manual reversion) |

Transition from `Verified` to a lower stage is allowed by the system but requires explicit user
action or a pipeline event. It is never done silently.

### AI-Proposed Fields

Fields produced by an AI pipeline (`source = AIAgent`) are treated as unconfirmed until reviewed.
The API response for any entity with unreviewed AI-proposed fields includes a flag
(`has_unreviewed_ai_fields: true`) at the envelope level so the UI can indicate review is needed.

The UI shows AI-proposed field values with a distinct provenance indicator (see `shared-entities.md`
in `docs/ui-design/`). An AI-proposed field value is applied to the entity record immediately — it
is not held in a staging area. The `reviewed` flag in `field_provenance` is the only indicator that
confirmation is pending.

---

## 4. Container vs. ContractSubject — Separation

These two concepts serve different purposes and must never be conflated:

| Aspect | Container | ContractSubject |
|--------|-----------|----------------|
| Defined by | User creates it voluntarily | Created as part of a contract record |
| Purpose | Cross-domain grouping for navigation and reporting | Structured descriptor of what a contract covers |
| Domain | Shared (cross-domain) | Contracts domain |
| Entity type | `containers` table | `contract_subjects` table |
| Membership | Via `ContainerMembership` join | Via `ProviderContract.contract_subject_id` FK |
| Overlap | Optional: a ContractSubject may be linked to a Container via `Link` | — |

A vehicle that has an insurance contract is both:
- A `ContractSubject` (the insured object, referenced by `ProviderContract`)
- Potentially a member of a `Container` named "BMW X5" via `ContainerMembership`

This overlap is intentional and handled by an explicit `Link` between the ContractSubject and the
Container when the user wants them connected. The system does not create this link automatically.

---

## 5. Open Decisions

The following decisions are deferred and require follow-up issues before implementation begins on
the affected features.

### 5.1 Audit Log

**Question:** Should field-level changes be tracked in a separate audit log table?

**Context:** The current `field_provenance` jsonb records the source of the most recent non-manual
derivation but does not track changes over time. If a user edits an AI-proposed value, the
provenance entry is removed and the original value is lost.

**Decision needed:** Define scope (which entities), retention period, and whether the audit log is
user-visible or admin-only.

### 5.2 Document Type Registry

**Question:** Should `document_type` be a free-text column or a foreign key to a structured
category table?

**Context:** Currently `documents.document_type` is `text`. This is flexible but makes filtering
and grouping inconsistent. A `document_categories` table (similar to `product_categories`) would
allow hierarchical grouping but adds maintenance burden.

**Decision needed:** Determine whether the household-specific hierarchy justifies the extra table,
or whether a curated enum with a freeform override is sufficient.

### 5.3 Stock History

**Question:** Should `StockEntry` maintain a history of quantity changes?

**Context:** The current schema stores only the current quantity, updated in place. There is no
record of when stock was added or depleted, or in what quantity. A `stock_movements` table would
provide this history but adds write complexity to every stock update.

**Decision needed:** Define whether household inventory tracking requires history and at what
granularity.

### 5.4 Portfolio Holdings Snapshot

**Question:** Should the system maintain a computed holdings snapshot for Depot accounts, or derive
current holdings entirely from `PortfolioEvent` records?

**Context:** Deriving current holdings from events is correct but expensive for large event
histories. A `portfolio_holdings` snapshot table (updated on each event) would provide O(1) reads
for "what do I currently own?" at the cost of a materialized view or trigger.

**Decision needed:** Define the acceptable read latency for portfolio views and whether a snapshot
table is warranted.

### 5.5 File Encryption at Rest

**Question:** Should files in `ocent-file-storage` be encrypted?

**Context:** The data persistence spec (`docs/arch/data.md`) lists this as an open question.
For self-hosted deployments on trusted hardware, filesystem encryption at the OS level may be
sufficient. Application-level encryption adds complexity but provides protection if the volume is
extracted.

**Decision needed:** Confirm whether application-level encryption is in scope or whether host-level
encryption is the expected solution.

### 5.6 Reconciliation Workflow

**Question:** How does a user formally reconcile a Transaction against a ContractVersion or
Document?

**Context:** `Transaction.status = Reconciled` is defined but the workflow to reach it is not.
Reconciliation could be: (a) automatic when a Link exists between the transaction and a contract,
(b) a manual user action on the transaction, or (c) a batch reconciliation UI.

**Decision needed:** Define the reconciliation UX and which system events trigger `Reconciled`
status.
