# Support transactions, CSV import, categorization, and splits

## Summary

Implement transaction ingestion and enrichment for finance accounts.

## Why

Transactions are the main operational dataset in the finance area and must support both easy import and later refinement.

## Scope

- manual transaction entry
- CSV import flow
- transaction categorization
- split transactions into multiple parts
- allow transactions and transaction splits to be linked to contracts
- support recurring transaction patterns or templates
- prepare transaction records for document linking and container assignment

## Acceptance Criteria

- users can create a transaction manually
- users can import transactions from CSV
- a transaction can be categorized
- a transaction can be split into multiple sub-records
- transactions and splits can be linked to contracts
- recurring transaction requirements are documented for implementation
- splits can be treated as first-class records for later linking