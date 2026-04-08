# ocent Domain Model — Overview

> Authoritative domain model for ocent: a self-hosted personal operations hub covering Finance,
> Documents, Storage, and Contracts for a single household.

---

## Principles

1. **UUID v4 everywhere.** All primary keys are UUID v4 stored as `uuid` in PostgreSQL. No
   sequential IDs, no prefixed slugs in the database. Display prefixes (e.g. `TXN-`, `DOC-`,
   `CTR-`) are a UI-only concern and never stored.

2. **Household-scoped, not user-scoped.** Every entity belongs to a `household_id`. Users are
   members of a household. The household is the root ownership boundary.

3. **Sparse field provenance.** Every enrichable entity carries a `field_provenance jsonb` column.
   Only non-manual field derivations have entries. Manually entered fields have no provenance
   record. Absence means manual.

4. **Domain-owned lifecycle status.** Each domain defines its own `status` enum for domain-specific
   semantics (e.g. `Transaction.status: Raw|Categorized|Reconciled|Disputed|Archived`). A shared
   `LifecycleStage` enum (`Raw|Enriched|Verified`) exists for cross-domain dashboards only.

5. **Containers are cross-domain groupings.** A Container groups entities from any domain. It uses
   a `ContainerMembership` join table with an `entity_type` discriminator. Containers are entirely
   separate from `ContractSubject`, which is a Contracts-domain concept.

6. **Links are explicit directed edges.** The `Link` table is a universal directed edge store. All
   cross-entity relationships that are not Container membership and not foreign keys are expressed
   as Link records.

7. **No multitenancy.** One deployment, one household. No tenant isolation logic. No row-level
   security beyond `household_id` checks in the application layer.

---

## Entity Map (ASCII)

```
┌──────────────────────────────────────────────────────────────────────────┐
│  SHARED                                                                  │
│                                                                          │
│  Household ──< User                                                      │
│       │                                                                  │
│       ├──< Container ──< ContainerMembership >── (any entity)            │
│       ├──< Tag ──< EntityTag >── (any entity)                            │
│       └──< Link (source_entity → target_entity, typed)                   │
│                                                                          │
│  FieldProvenance (jsonb on each enrichable entity)                       │
│  LifecycleStage enum: Raw | Enriched | Verified                          │
│  Source enum: ManualEntry | CsvImport | BulkUpload | OCR | AIAgent       │
└──────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────┐   ┌─────────────────────────┐
│  FINANCE                │   │  DOCUMENTS              │
│                         │   │                         │
│  FinanceAccount         │   │  Document               │
│  Transaction            │   │    └─ DocumentVersion   │
│    └─ TransactionSplit  │   │         └─ DocumentFile │
│  TransactionCategory    │   └─────────────────────────┘
│  RecurringPattern       │
│  Asset                  │   ┌─────────────────────────┐
│    └─ ValueHistoryEntry │   │  STORAGE                │
│  Liability              │   │                         │
│  Receivable             │   │  ProductCategory        │
│  PortfolioEvent         │   │  Product                │
│  PensionContribution    │   │    └─ ProductAlias      │
│  PensionStatusReport    │   │  StorageLocation        │
└─────────────────────────┘   │  StockEntry             │
                              │  PriceRecord            │
┌─────────────────────────┐   └─────────────────────────┘
│  CONTRACTS              │
│                         │
│  ContractSubject        │
│  ProviderContract       │
│    └─ ContractVersion   │
│         ├─ ConditionPhase│
│         └─ CancellationRule│
└─────────────────────────┘
```

---

## File Index

| File | Contents |
|------|----------|
| `shared-entities.md` | Household, User, Ownership, Container, Tag, Link, Source, FieldProvenance, LifecycleStage |
| `finance.md` | FinanceAccount, Transaction, TransactionSplit, TransactionCategory, RecurringPattern, Asset, Liability, Receivable, PortfolioEvent, PensionContribution, PensionStatusReport |
| `documents.md` | Document, DocumentVersion, DocumentFile |
| `storage-contracts.md` | ProductCategory, Product, ProductAlias, StorageLocation, StockEntry, PriceRecord, ContractSubject, ProviderContract, ContractVersion, ConditionPhase, CancellationRule |
| `cross-domain-rules.md` | Link rules, Ownership rules, Provenance rules, Open decisions |
