# ocent

ocent is a proposed self-hosted personal operations hub for private individuals who want one place to manage, track, and analyze finances, documents, storage, and contracts.

This README captures the current product vision and suggested scope based on the idea definition. It intentionally stays technology-neutral where implementation decisions are still open.

## Vision

Private life administration is fragmented. Financial records live in banking apps and spreadsheets, documents live in folders and mailboxes, contracts are hard to monitor, and related information is rarely connected.

ocent aims to become the central system of record for a private person or household by combining:

- financial tracking
- document management
- storage and item organization
- contract oversight
- cross-domain grouping and analysis
- workflow automation with tasks and AI agents

## Product Goals

- Give users a single, structured place for personal administrative data.
- Preserve links between financial events, documents, contracts, and stored items.
- Support both raw data capture and later enrichment.
- Enable yearly or case-based grouping across domains, for example `Income Tax 2026`.
- Allow deterministic workflows and agent-assisted pipelines for repetitive work.
- Provide a dedicated dashboard for each major domain.
- Provide an all-time cross-finance dashboard across money accounts, depots, and pension insurance records.
- Provide a net worth view and a forward-looking liquidity view.
- Provide active budgeting and financial planning capabilities.
- Provide structured tax preparation workflows, strong traceability, and cross-domain search.

## Core Concepts

### Domains

ocent is organized around several major domains:

- Finance
- Documents
- Storage
- Contracts

Each domain should work on its own, but also connect cleanly to the others.

Each major domain should also have its own dedicated dashboard so users can review status, open items, trends, and relevant KPIs without having to navigate through raw records first.

The application should also support household-aware ownership and visibility so records can be private, shared, or read-only where needed.

### Containers

Containers are user-defined cross-domain groupings. A container can collect related records from different areas of the system.

Examples:

- `Income Tax 2026`
- `Apartment Purchase`
- `Car Ownership`
- `Insurance Claim 2027`

Containers are intended to help users group data by purpose, event, or time period instead of by technical module.

### Pipelines

Pipelines are user-defined workflows for sequential and parallel execution of tasks, prompts, and agents.

Examples:

- OCR a document and populate suggested fields
- extract correspondents, dates, and document types
- propose transaction categories after CSV import
- link a dividend statement to the matching dividend event
- parse a receipt and update product quantities in inventory
- prepare records relevant for annual tax review

Pipelines should support classic task automation and AI-assisted processing.

Across all automated and manual flows, important fields should preserve their origin, for example whether a value was entered manually, imported, extracted by OCR, or proposed by AI.

## Functional Scope

### Finance

The finance area should allow a user to maintain multiple types of money accounts, including:

- bank accounts
- P2P accounts
- cash accounts
- virtual accounts

Users should be able to ingest transactions:

- manually, one by one
- by CSV import
- by additional import channels where applicable, such as bank export formats or bulk import flows

Transactions should support:

- categorization
- split booking across multiple categories or purposes
- later linking to supporting documents
- linking to one or more related contracts where applicable
- recurring transaction patterns where applicable
- inclusion in containers

In addition to money accounts and transactions, the finance area should support pension tracking, portfolio tracking, liabilities, receivables, and real estate.

#### Liabilities

Users should be able to maintain liabilities such as loans and other obligations, including:

- principal or outstanding balance
- interest terms
- repayment plans
- current status and history

#### Receivables

Users should be able to maintain receivables such as private loans granted to others, including:

- original amount
- repayment expectations
- received repayments
- current outstanding amount

#### Real Estate

Users should be able to maintain real estate records, including:

- property master data
- acquisition-related information
- financing links
- value history where available
- related documents and contracts

Valuation logic should be explicit for assets and obligations that are not simple account balances, so net worth views remain interpretable.

#### Pension Insurance

Users should be able to maintain pension insurance records, including:

- contracts or products
- contributions or payments
- status reports

Pension insurance views and dashboards should support KPI-driven analysis where possible. This includes, at minimum:

- XIRR
- TTWROR
- absolute delta
- relative delta
- estimated pension income
- guaranteed pension income

#### Investment Portfolios

Users should be able to maintain depots and related events, including:

- buys
- sells
- dividends

Finance dashboards should support KPI-driven analysis. This includes, at minimum:

- XIRR
- TTWROR
- absolute delta
- relative delta

Depot and portfolio dashboards should additionally support investment-specific KPIs, including:

- dividend yield
- income development from dividends
- performance views per depot and across depots
- projected dividend income
- projected interest income

The finance area should also support a consolidated dashboard across all time for:

- money accounts
- depots
- pension insurance records
- liabilities
- receivables
- real estate

This consolidated dashboard should make it possible to review KPIs across the full financial picture instead of only inside isolated sub-domains.

The finance area should also support:

- net worth views across all relevant assets and liabilities
- forward-looking liquidity forecasts for upcoming days, weeks, or months
- recurring transaction management for expected inflows and outflows
- variance analysis between expected and actual financial events
- budgeting and planning across months, years, categories, and savings goals

The finance area should also provide a dedicated retirement dashboard that combines:

- current pension insurance information
- estimated pension income
- guaranteed pension income
- projected dividend income
- projected interest income

This dashboard should help users understand their retirement-oriented income outlook across pension and capital income sources.

## Documents

The document area should support low-friction ingestion first and enrichment later.

Users should be able to upload one or multiple documents without having to provide metadata immediately.

Document intake should also support additional import channels beyond single manual upload, for example bulk imports, drag-and-drop collections, email-based intake, or PDF batches.

Document upload should remain fast and should not wait for full background pipeline completion.

At upload time, the application should perform synchronous intake validation where appropriate, for example:

- file readability
- allowed file type checks
- technical integrity checks
- technical duplicate detection for exact or near-exact file duplicates

Deeper document processing should then continue asynchronously in the background, for example:

- OCR
- metadata extraction
- classification
- linking suggestions
- agent-driven review or enrichment steps

The application should also distinguish between:

- technical duplicates that can be blocked or warned about during upload
- likely business duplicates that should usually be flagged for later review instead of blocking intake

A document should later be enrichable with fields such as:

- name
- type
- correspondent
- date
- content

Documents should be linkable to relevant domain records, including:

- a transaction
- a transaction split
- a contract
- a pension status report
- a buy event
- a sell event
- a dividend event

The document area should also expose its own dashboard, for example to highlight recently uploaded files, unenriched documents, unlinked evidence, and pipeline suggestions.

Documents should also support versioning so later revisions of the same document can be tracked without losing history.

The application should also provide strong global search across documents, contracts, transactions, products, merchants, and containers.

## Storage

The storage area should evolve into a catalog and inventory system for physical goods, consumables, and stored household items.

The catalog should support hierarchical product structures, for example:

- `Dry Pasta`
- `Dry Spatzle`
- `Tress Spatzle`
- `Tress Swabian Spatzle`

This should make it possible to see both detailed item availability and aggregated availability at higher product levels.

The catalog and storage system should support:

- canonical products with aliases and OCR variants
- hierarchical product classification
- merchant-aware product and price history
- current quantity tracking
- automated stock updates based on receipts or invoices where possible
- future recipe or meal matching based on current stock

Storage locations should also support hierarchy, for example:

- `Basement > Shelf 2 > Compartment 5`
- `Kitchen > Cabinet 1`

This should make it possible to track both where an item is stored and how much is available at each level.

The storage area should also have its own dashboard to surface inventory status, recently changed items, and container-based overviews.

The storage dashboard should additionally support:

- low-stock warnings
- out-of-stock warnings
- highlighted special offers
- quantity summaries by product group
- visibility into which meals or recipes are possible with the current stock model

## Contracts

The contracts area is intended to track private agreements and recurring obligations. Detailed requirements are still open, but it is part of the intended product scope.

Contracts should be linkable to one or more related documents and to one or more related transactions.

For long-lived subjects such as `Electricity for Apartment A`, the application should support a higher-level contract container or contract context.

This contract container should make it possible to group:

- multiple provider contracts over time
- contract versions or revisions within one provider contract
- related transactions
- related documents

Contracts should also support versioning or contract revisions so that changing conditions are stored historically instead of overwriting previous values.

This should include price phases or condition phases where applicable, for example:

- electricity working price changes
- monthly base price changes
- tariff revisions
- contract renewals with new terms

Contracts should also support cancellation-related logic, including:

- current cancellation period
- next possible cancellation date
- rule-based changes of cancellation periods after defined contract phases
- reminders and warnings before important cancellation deadlines

This should make it possible to use contracts as the basis for expected recurring costs and to compare them against actual financial movements.

It should also make it possible to analyze the full history of one real-world subject, such as apartment electricity, across multiple providers without losing continuity.

The contracts area should also have its own dashboard to surface active contracts, renewal dates, due dates, and related documents.

Where contracts contain changing prices or terms, the contract area should also make it possible to review their historical development over time.

The contract area should also surface upcoming cancellation deadlines and significant rule changes that affect cancellation timing.

Notifications should not only exist inside dashboards. The application should also support a structured notification and escalation model with priorities, due dates, and delivery channels where appropriate.

## Tax Process

In addition to cross-domain containers such as `Income Tax 2026`, the application should support a structured tax process.

This should make it possible to manage:

- tax-relevant transactions, documents, and contracts
- completeness of evidence
- review status for a tax case
- tax-related deadlines and tasks
- prepared exports or handoff views for external processing

## Cross-Domain Linking

One of the main differentiators of ocent is that records should not stay isolated inside domain silos.

Examples of intended links:

- a bank transaction linked to the invoice document that explains it
- a dividend event linked to the broker statement document
- a pension status report linked to the underlying insurance record
- a tax container that groups transactions, documents, and contracts relevant for one year

## Dashboard and KPI Expectations

ocent should provide both domain-specific dashboards and consolidated views where appropriate.

For finance in particular, users should be able to analyze KPIs:

- over all time
- per account or depot
- per pension insurance record
- per liability, receivable, or property where applicable
- across all money accounts combined
- across all depots combined
- across money accounts, depots, pension insurance records, liabilities, receivables, and real estate in one consolidated financial view
- in a retirement-oriented dashboard that combines pension and projected capital income

Finance should also support:

- net worth development over time
- liquidity forecasts based on expected inflows and outflows
- recurring transaction views
- variance views between contractual or expected values and actual bookings
- budget versus actual views across categories and planning periods

Tax-related views should also support:

- case completeness
- review status
- deadline visibility
- prepared export or handoff states

For catalog and storage in particular, users should be able to analyze:

- current quantity per product
- aggregated quantity per product hierarchy level
- quantity by storage location
- recent stock changes based on receipts and invoices
- price history per product and merchant
- products that are low in stock or currently attractive due to offers

Across all domains, the application should also provide a central task and deadline center that aggregates:

- reminders
- warnings
- review tasks
- missing links or enrichment steps
- upcoming contract actions
- inventory actions

Across all domains, the application should also support:

- global search and knowledge retrieval across linked records
- field-level data provenance and auditability
- notification delivery and escalation behavior
- household-aware access and ownership models

## Suggested Product Principles

The following principles are proposed and should be validated during planning:

- privacy-first design for sensitive personal data
- strong traceability between records and evidence documents
- self-hosted deployment as the primary operating model
- modular domain boundaries with a shared linking model
- automation-friendly architecture with human review where needed

## Suggested Delivery Approach

Because the current scope is broad, a phased implementation approach is recommended.

### Phase 1

- shared domain model
- containers
- finance accounts
- transactions with manual entry and CSV import
- document upload and enrichment
- cross-linking between transactions and documents

### Phase 2

- pension insurance tracking
- portfolio tracking for buys, sells, and dividends
- storage and contract domain foundations
- pipeline engine for tasks and agents

### Phase 3

- dashboards across all domains
- analytics across all domains
- deeper automation and agent workflows
- domain-specific assistant features

## Initial Non-Goals

The following are intentionally not fixed yet:

- final tech stack
- final UI framework
- accounting-grade double-entry design
- banking integrations beyond CSV import
- OCR engine choice
- agent framework choice

## Issue Planning

Matching issue drafts are available in `.github/issue-drafts/` so they can be turned into GitHub issues with minimal rework.

## Status

Concept stage. Product scope is defined at a vision level and should now be refined into concrete epics, domain models, and implementation milestones.