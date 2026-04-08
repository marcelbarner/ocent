# Finance Domain

The Finance domain covers all monetary accounts, transactions, investments, real assets, debts,
and pension tracking for the household.

---

## 1. FinanceAccount

Represents a financial account of any type held by the household. Type-specific data is stored in
`metadata jsonb` to avoid a wide table with mostly-null columns.

**Table name:** `finance_accounts`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `name` | `text` | NOT NULL | User-given account name |
| `account_type` | `account_type` | NOT NULL | Enum (see below) |
| `institution_name` | `text` | nullable | Bank or provider name |
| `account_number_masked` | `text` | nullable | Last 4 digits or IBAN suffix |
| `currency` | `char(3)` | NOT NULL, default `EUR` | ISO 4217 |
| `is_active` | `boolean` | NOT NULL, default `true` | Closed accounts remain for history |
| `opened_at` | `date` | nullable | Account opening date |
| `closed_at` | `date` | nullable | Account closure date |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `metadata` | `jsonb` | NOT NULL, default `'{}'` | Type-specific fields |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | Per-field provenance |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `account_type`:** `Bank | P2P | Cash | Virtual | Depot | PensionInsurance`

### metadata jsonb by account_type

| account_type | Relevant metadata fields |
|-------------|--------------------------|
| `Bank` | `iban`, `bic`, `bank_code` |
| `P2P` | `platform_name`, `platform_url` |
| `Cash` | `location_note` (e.g. "wallet", "safe") |
| `Virtual` | `provider`, `purpose` (e.g. "PayPal balance") |
| `Depot` | `depot_number`, `broker_name`, `depot_type` (e.g. "ETF", "Aktien") |
| `PensionInsurance` | `contract_number`, `insurer_name`, `pension_type` (e.g. "Riester", "bAV"), `guaranteed_pension_amount`, `current_pension_amount` |

---

## 2. Transaction

A single financial movement on a FinanceAccount.

**Table name:** `transactions`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `finance_account_id` | `uuid` | FK → `finance_accounts.id`, NOT NULL | |
| `booking_date` | `date` | NOT NULL | Date the bank posted the transaction |
| `value_date` | `date` | nullable | Value date (Valuta) as reported by bank |
| `amount` | `numeric(18,4)` | NOT NULL | Positive = credit, negative = debit |
| `currency` | `char(3)` | NOT NULL | ISO 4217 |
| `description` | `text` | nullable | Raw transaction description from bank |
| `counterpart_name` | `text` | nullable | Name of the other party |
| `counterpart_iban` | `text` | nullable | IBAN of the other party |
| `category_id` | `uuid` | FK → `transaction_categories.id`, nullable | |
| `status` | `transaction_status` | NOT NULL, default `Raw` | |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `is_split_parent` | `boolean` | NOT NULL, default `false` | True when this transaction has splits |
| `external_id` | `text` | nullable | ID from import source (deduplication) |
| `source` | `source_type` | NOT NULL, default `ManualEntry` | How the record was created |
| `notes` | `text` | nullable | User notes |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `transaction_status`:** `Raw | Categorized | Reconciled | Disputed | Archived`

| Status | Meaning |
|--------|---------|
| `Raw` | Imported or entered, not yet reviewed |
| `Categorized` | Category assigned (manually or by AI), not fully reconciled |
| `Reconciled` | Matched to a document, contract, or counterpart record |
| `Disputed` | Flagged for investigation (wrong amount, duplicate, fraud) |
| `Archived` | Closed, no further action needed |

**Index:** `(finance_account_id, booking_date DESC)` — primary query pattern.
**Index:** `(household_id, booking_date DESC)` — household-wide transaction list.
**Unique index:** `(finance_account_id, external_id)` where `external_id IS NOT NULL` — deduplication.

---

## 3. TransactionSplit

Divides a parent transaction into labelled portions. The sum of all splits must equal the parent
`amount`. Splits are for categorization only — they do not generate separate bank movements.

**Table name:** `transaction_splits`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `transaction_id` | `uuid` | FK → `transactions.id`, NOT NULL | Must have `is_split_parent = true` |
| `amount` | `numeric(18,4)` | NOT NULL | Portion of parent transaction |
| `category_id` | `uuid` | FK → `transaction_categories.id`, nullable | |
| `description` | `text` | nullable | Split-specific description |
| `sort_order` | `integer` | NOT NULL, default `0` | Display ordering |
| `created_at` | `timestamptz` | NOT NULL | |

The application enforces: `SUM(split.amount) = parent.amount` on every write to splits.

---

## 4. TransactionCategory

Hierarchical category tree for transactions. A category has an optional parent, making it a tree
of unlimited depth (though the UI limits display to three levels).

**Table name:** `transaction_categories`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `parent_id` | `uuid` | FK → `transaction_categories.id`, nullable | Null = root category |
| `name` | `text` | NOT NULL | Display name |
| `is_income` | `boolean` | NOT NULL, default `false` | Income vs. expense |
| `is_tax_relevant` | `boolean` | NOT NULL, default `false` | Marks categories relevant to tax reports |
| `sort_order` | `integer` | NOT NULL, default `0` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(household_id, parent_id, name)` — no duplicate sibling names.

System-supplied default categories are seeded at household creation. User-created categories live
alongside them in the same table, distinguished by `created_by_user_id`.

---

## 5. RecurringPattern

Describes an expected regular transaction flow — e.g. a monthly salary or a quarterly insurance
premium. Can optionally link to a ContractVersion.

**Table name:** `recurring_patterns`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `finance_account_id` | `uuid` | FK → `finance_accounts.id`, NOT NULL | |
| `name` | `text` | NOT NULL | Human label, e.g. "Netflix monthly" |
| `frequency` | `recurrence_frequency` | NOT NULL | Enum (see below) |
| `expected_amount` | `numeric(18,4)` | nullable | Expected amount per occurrence |
| `currency` | `char(3)` | NOT NULL, default `EUR` | |
| `category_id` | `uuid` | FK → `transaction_categories.id`, nullable | |
| `linked_contract_version_id` | `uuid` | FK → `contract_versions.id`, nullable | Backing contract |
| `first_occurrence_date` | `date` | nullable | |
| `last_occurrence_date` | `date` | nullable | Null = ongoing |
| `is_active` | `boolean` | NOT NULL, default `true` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `recurrence_frequency`:** `Daily | Weekly | Biweekly | Monthly | Quarterly | Semiannual | Annual | Irregular`

Matching incoming transactions to a RecurringPattern is handled by the finance pipeline and
recorded via the `Link` table (`link_type = recurring_contract`).

---

## 6. Asset (RealEstate)

Represents a high-value owned asset. The initial implementation covers RealEstate. Other asset
types (vehicle, equipment) are modelled via `asset_type` and `metadata jsonb`.

**Table name:** `assets`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `name` | `text` | NOT NULL | Human label, e.g. "Apartment Hauptstraße" |
| `asset_type` | `asset_type` | NOT NULL | Enum: `RealEstate | Vehicle | Other` |
| `acquisition_date` | `date` | nullable | Date of purchase or acquisition |
| `acquisition_cost` | `numeric(18,4)` | nullable | Purchase price |
| `acquisition_currency` | `char(3)` | NOT NULL, default `EUR` | |
| `current_value` | `numeric(18,4)` | nullable | Most recent estimated value |
| `current_value_date` | `date` | nullable | Date of the most recent valuation |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `metadata` | `jsonb` | NOT NULL, default `'{}'` | Type-specific data |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**metadata by asset_type:**

| asset_type | Relevant metadata fields |
|-----------|--------------------------|
| `RealEstate` | `address`, `land_registry_number`, `usable_area_sqm`, `plot_area_sqm`, `property_type` (Apartment, House, Land) |
| `Vehicle` | `vin`, `make`, `model`, `year`, `license_plate` |
| `Other` | `description` |

### ValueHistoryEntry

Append-only log of asset valuations over time.

**Table name:** `asset_value_history`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `asset_id` | `uuid` | FK → `assets.id`, NOT NULL | |
| `valuation_date` | `date` | NOT NULL | |
| `value` | `numeric(18,4)` | NOT NULL | |
| `currency` | `char(3)` | NOT NULL | |
| `source` | `source_type` | NOT NULL | |
| `notes` | `text` | nullable | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(asset_id, valuation_date)` — one valuation per asset per day.

---

## 7. Liability

A debt owed by the household — loan, mortgage, overdraft.

**Table name:** `liabilities`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `name` | `text` | NOT NULL | E.g. "Santander Car Loan" |
| `liability_type` | `liability_type` | NOT NULL | Enum: `Loan | Mortgage | Overdraft | Other` |
| `lender_name` | `text` | nullable | |
| `original_amount` | `numeric(18,4)` | nullable | Principal at origination |
| `outstanding_balance` | `numeric(18,4)` | NOT NULL | Current balance (manually updated or synced) |
| `currency` | `char(3)` | NOT NULL, default `EUR` | |
| `interest_rate_percent` | `numeric(6,4)` | nullable | Annual interest rate |
| `start_date` | `date` | nullable | |
| `end_date` | `date` | nullable | Planned payoff date |
| `linked_asset_id` | `uuid` | FK → `assets.id`, nullable | Securing asset (for Mortgage) |
| `linked_finance_account_id` | `uuid` | FK → `finance_accounts.id`, nullable | Repayment account |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

---

## 8. Receivable

Money owed to the household by a third party. Tracks informal debts and expected reimbursements.

**Table name:** `receivables`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `debtor_name` | `text` | NOT NULL | Who owes the money |
| `description` | `text` | nullable | What for |
| `amount` | `numeric(18,4)` | NOT NULL | Amount originally owed |
| `currency` | `char(3)` | NOT NULL, default `EUR` | |
| `due_date` | `date` | nullable | |
| `amount_received` | `numeric(18,4)` | NOT NULL, default `0` | Accumulated received payments |
| `status` | `receivable_status` | NOT NULL, default `Open` | Enum: `Open | PartiallyPaid | Paid | WrittenOff` |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

---

## 9. PortfolioEvent

A recorded event within a Depot account: purchase, sale, dividend, corporate action.

**Table name:** `portfolio_events`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `finance_account_id` | `uuid` | FK → `finance_accounts.id`, NOT NULL | Must be account_type = Depot |
| `event_type` | `portfolio_event_type` | NOT NULL | Enum (see below) |
| `event_date` | `date` | NOT NULL | |
| `isin` | `char(12)` | nullable | International Securities Identification Number |
| `ticker` | `text` | nullable | |
| `instrument_name` | `text` | nullable | Human-readable name, e.g. "iShares Core MSCI World" |
| `quantity` | `numeric(18,8)` | nullable | Shares/units |
| `price_per_unit` | `numeric(18,6)` | nullable | Price at time of event |
| `total_amount` | `numeric(18,4)` | NOT NULL | Total value of event (positive = inflow) |
| `currency` | `char(3)` | NOT NULL | |
| `fees` | `numeric(18,4)` | nullable | Transaction costs |
| `tax_withheld` | `numeric(18,4)` | nullable | Withholding tax |
| `linked_transaction_id` | `uuid` | FK → `transactions.id`, nullable | Corresponding cash movement |
| `source` | `source_type` | NOT NULL, default `ManualEntry` | |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |

**Enum `portfolio_event_type`:** `Buy | Sell | Dividend | CorporateAction | Transfer | Fee`

---

## 10. PensionContribution

A single contribution made to a PensionInsurance account.

**Table name:** `pension_contributions`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `finance_account_id` | `uuid` | FK → `finance_accounts.id`, NOT NULL | Must be account_type = PensionInsurance |
| `contribution_date` | `date` | NOT NULL | |
| `amount` | `numeric(18,4)` | NOT NULL | |
| `currency` | `char(3)` | NOT NULL | |
| `contribution_type` | `text` | NOT NULL | E.g. `EmployeeContribution`, `EmployerContribution`, `StateBonus` |
| `linked_transaction_id` | `uuid` | FK → `transactions.id`, nullable | |
| `source` | `source_type` | NOT NULL, default `ManualEntry` | |
| `created_at` | `timestamptz` | NOT NULL | |

---

## 11. PensionStatusReport

A point-in-time snapshot from an annual pension statement.

**Table name:** `pension_status_reports`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `finance_account_id` | `uuid` | FK → `finance_accounts.id`, NOT NULL | |
| `report_date` | `date` | NOT NULL | Date of the statement |
| `current_capital` | `numeric(18,4)` | nullable | Accumulated capital at report date |
| `projected_monthly_pension` | `numeric(18,4)` | nullable | Projected pension at retirement |
| `guaranteed_monthly_pension` | `numeric(18,4)` | nullable | Guaranteed minimum |
| `currency` | `char(3)` | NOT NULL | |
| `projected_retirement_date` | `date` | nullable | |
| `linked_document_id` | `uuid` | FK → `documents.id`, nullable | Source document (annual statement) |
| `source` | `source_type` | NOT NULL, default `ManualEntry` | |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(finance_account_id, report_date)` — one report per account per day.
