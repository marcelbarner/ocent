# Build finance account and finance entity foundations

## Summary

Implement the finance foundation for personal money accounts and adjacent finance entities.

## Why

Finance entity modeling is the base for transaction capture, portfolio tracking, liability tracking, property tracking, and personal financial analysis.

## Scope

- support bank accounts
- support P2P accounts
- support cash accounts
- support virtual accounts
- define how liabilities, receivables, and real estate attach to the broader finance model
- define shared account fields and account-specific extensions

## Acceptance Criteria

- users can create and edit the supported account types
- accounts have consistent core metadata
- liabilities, receivables, and real estate fit the same finance model cleanly
- account types are represented in a way that later transaction and analytics features can reuse
- the design supports extension without breaking existing records