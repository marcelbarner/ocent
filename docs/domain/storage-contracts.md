# Storage and Contracts Domains

---

## Part A: Storage Domain

The Storage domain tracks household inventory: products that are owned or consumed, where they are
stored, current stock levels, and price history across merchants.

---

### 1. ProductCategory

A hierarchical category tree for products. Categories are household-specific and have unlimited
depth, though the UI constrains display to three levels.

**Table name:** `product_categories`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `parent_id` | `uuid` | FK → `product_categories.id`, nullable | Null = root category |
| `name` | `text` | NOT NULL | Display name |
| `sort_order` | `integer` | NOT NULL, default `0` | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(household_id, parent_id, name)`

---

### 2. Product

A product that the household tracks. Products are identified by unique names within the household.
Aliases allow multiple names (e.g. brand vs. generic) for the same product.

**Table name:** `products`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `category_id` | `uuid` | FK → `product_categories.id`, nullable | |
| `name` | `text` | NOT NULL | Canonical product name |
| `barcode` | `text` | nullable | EAN/UPC for scanner input |
| `unit` | `text` | NOT NULL, default `piece` | Unit of measure: `piece`, `kg`, `g`, `l`, `ml`, `pack` |
| `default_quantity` | `numeric(10,3)` | NOT NULL, default `1` | Default quantity when adding stock |
| `is_consumable` | `boolean` | NOT NULL, default `true` | Consumables deplete; non-consumables are assets |
| `notes` | `text` | nullable | |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(household_id, name)`

### 2a. ProductAlias

Alternative names for a product. Used for barcode lookup, import matching, and search.

**Table name:** `product_aliases`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `product_id` | `uuid` | FK → `products.id`, NOT NULL | |
| `alias` | `text` | NOT NULL | Alternative name |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(product_id, alias)`

---

### 3. StorageLocation

A physical or logical location where products are stored. Locations are hierarchical — a house can
contain rooms, rooms can contain shelves.

**Table name:** `storage_locations`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `parent_id` | `uuid` | FK → `storage_locations.id`, nullable | Null = top-level location |
| `name` | `text` | NOT NULL | E.g. "Kitchen", "Pantry Shelf 2", "Basement Freezer" |
| `location_type` | `text` | NOT NULL, default `Generic` | `Generic | Room | Shelf | Freezer | Fridge | Cabinet | Box` |
| `sort_order` | `integer` | NOT NULL, default `0` | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(household_id, parent_id, name)`

---

### 4. StockEntry

The current quantity of a specific product at a specific storage location. One row per
(product, location) pair. Quantity is updated in place — history is not kept (use PriceRecord
history for price tracking; stock history is a future feature).

**Table name:** `stock_entries`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `product_id` | `uuid` | FK → `products.id`, NOT NULL | |
| `storage_location_id` | `uuid` | FK → `storage_locations.id`, NOT NULL | |
| `quantity` | `numeric(10,3)` | NOT NULL, default `0` | Current quantity in product's unit |
| `minimum_quantity` | `numeric(10,3)` | nullable | Alert threshold |
| `expiry_date` | `date` | nullable | Expiry of the current batch |
| `updated_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(product_id, storage_location_id)`

---

### 5. PriceRecord

Historical price observations for a product at a specific merchant. Used to track price changes
over time and identify the best current source.

**Table name:** `price_records`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `product_id` | `uuid` | FK → `products.id`, NOT NULL | |
| `merchant_name` | `text` | NOT NULL | Store or online merchant name |
| `price` | `numeric(10,4)` | NOT NULL | Price per unit |
| `currency` | `char(3)` | NOT NULL, default `EUR` | |
| `observed_at` | `date` | NOT NULL | Date this price was recorded |
| `source` | `source_type` | NOT NULL, default `ManualEntry` | |
| `notes` | `text` | nullable | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(product_id, merchant_name, observed_at)`

---

## Part B: Contracts Domain

The Contracts domain manages long-running legal and service agreements: insurance policies,
subscriptions, rental agreements, loans, and any other contract that the household holds with a
provider.

**Key separation:** `ContractSubject` is a Contracts-domain entity that represents the real-world
subject of a contract (e.g. "BMW X5 VIN:...") and is distinct from `Container`. A Container is a
cross-domain grouping owned by the user. A ContractSubject is a structured descriptor of what a
contract covers, owned by the contract.

---

### 6. ContractSubject

A real-world subject that a contract covers. Multiple contracts can share the same subject
(e.g. multiple insurance policies covering the same vehicle).

**Table name:** `contract_subjects`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `subject_type` | `contract_subject_type` | NOT NULL | Enum (see below) |
| `name` | `text` | NOT NULL | Human label, e.g. "BMW X5 2022", "Apartment Hauptstraße" |
| `description` | `text` | nullable | |
| `metadata` | `jsonb` | NOT NULL, default `'{}'` | Subject-type-specific structured data |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `contract_subject_type`:**
`Person | Vehicle | RealEstate | Device | Animal | GeneralAsset | Service | Other`

**metadata by subject_type:**

| subject_type | Relevant metadata fields |
|-------------|--------------------------|
| `Vehicle` | `vin`, `make`, `model`, `year`, `license_plate` |
| `RealEstate` | `address`, `property_type` |
| `Device` | `serial_number`, `manufacturer`, `model` |
| `Person` | `relation` (e.g. "self", "partner") |

A ContractSubject is not the same as a Container. It is the insured or contracted object, not a
user-defined grouping. The two may overlap conceptually (e.g. the vehicle is both a ContractSubject
and a Container member) but are managed independently. A ContractSubject may be linked to a
Container via the `Link` table if the user wants the connection explicit.

---

### 7. ProviderContract

The master contract record with a specific provider.

**Table name:** `provider_contracts`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `household_id` | `uuid` | FK, NOT NULL | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `visibility` | `visibility` | NOT NULL | |
| `write_restricted` | `boolean` | NOT NULL, default `false` | |
| `name` | `text` | NOT NULL | User-given name, e.g. "Allianz Kfz-Haftpflicht" |
| `provider_name` | `text` | NOT NULL | Counterparty name |
| `contract_number` | `text` | nullable | Provider-assigned number |
| `contract_type` | `contract_type` | NOT NULL | Enum (see below) |
| `contract_subject_id` | `uuid` | FK → `contract_subjects.id`, nullable | What the contract covers |
| `lifecycle_stage` | `lifecycle_stage` | NOT NULL, default `Raw` | |
| `field_provenance` | `jsonb` | NOT NULL, default `'{}'` | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `contract_type`:**
`Insurance | Subscription | Rental | Loan | Mortgage | Utility | Maintenance | Employment | Other`

---

### 8. ContractVersion

A specific set of terms that applied during a defined period. When a contract is renewed with
changed conditions, a new ContractVersion is created. The active version is the one with no
`end_date` or the latest `start_date`.

**Table name:** `contract_versions`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `provider_contract_id` | `uuid` | FK → `provider_contracts.id`, NOT NULL | |
| `version_number` | `integer` | NOT NULL | Monotonically increasing per contract |
| `start_date` | `date` | NOT NULL | When these terms took effect |
| `end_date` | `date` | nullable | Null = currently active version |
| `status` | `contract_version_status` | NOT NULL, default `Draft` | |
| `notes` | `text` | nullable | |
| `created_by_user_id` | `uuid` | FK, NOT NULL | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Enum `contract_version_status`:** `Draft | Active | Superseded | Cancelled | Expired`

**Unique constraint:** `(provider_contract_id, version_number)`

**Rule:** Only one ContractVersion per provider_contract may have `status = Active` at any time.
This is enforced by a partial unique index: `UNIQUE (provider_contract_id) WHERE status = 'Active'`.

---

### 9. ConditionPhase

A term or payment condition within a ContractVersion. A contract version may have multiple
sequential phases (e.g. introductory rate for 12 months, then standard rate).

**Table name:** `condition_phases`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `contract_version_id` | `uuid` | FK → `contract_versions.id`, NOT NULL | |
| `phase_order` | `integer` | NOT NULL | Sequence within the version |
| `label` | `text` | nullable | E.g. "Introductory", "Standard" |
| `start_date` | `date` | NOT NULL | |
| `end_date` | `date` | nullable | Null = open-ended or last phase |
| `amount` | `numeric(18,4)` | nullable | Payment amount per interval |
| `currency` | `char(3)` | NOT NULL, default `EUR` | |
| `payment_interval` | `recurrence_frequency` | nullable | Frequency of payment |
| `payment_day` | `integer` | nullable | Day of month payment is due |
| `notes` | `text` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |

**Unique constraint:** `(contract_version_id, phase_order)`

---

### 10. CancellationRule

The cancellation terms for a ContractVersion. Defines the notice period, deadline, and renewal
behaviour.

**Table name:** `cancellation_rules`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | `uuid` | PK | |
| `contract_version_id` | `uuid` | FK → `contract_versions.id`, NOT NULL, UNIQUE | One rule per version |
| `notice_period_days` | `integer` | nullable | Calendar days notice required |
| `notice_period_months` | `integer` | nullable | Months notice required (alternative to days) |
| `cancellation_deadline` | `date` | nullable | Computed or manually entered hard deadline |
| `auto_renews` | `boolean` | NOT NULL, default `false` | Does the contract renew automatically? |
| `renewal_duration_months` | `integer` | nullable | Length of each renewal period |
| `next_renewal_date` | `date` | nullable | Computed next automatic renewal date |
| `cancellation_form_required` | `boolean` | NOT NULL, default `false` | Must be cancelled in writing |
| `notes` | `text` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

`notice_period_days` and `notice_period_months` are mutually exclusive — exactly one should be
set when the contract has a notice period. The application derives `cancellation_deadline` from
these fields and the `end_date` of the ContractVersion, but stores the computed result for
efficient querying (upcoming cancellation deadline dashboard).
