# Add liabilities, receivables, and real estate to finance

## Summary

Extend the finance domain to cover liabilities, receivables, and real estate as first-class financial records.

## Why

Personal finance is incomplete without debts, claims against others, and property-related assets and obligations.

## Scope

- model liabilities such as loans and similar obligations
- model receivables such as private loans granted to other people
- model real estate records and their key finance relationships
- define how these entities participate in consolidated finance dashboards
- define how these entities link to documents, contracts, and containers

## Acceptance Criteria

- liabilities can be represented with outstanding balance and repayment context
- receivables can be represented with expected and received repayments
- real estate can be represented with core metadata and finance relationships
- all three entity groups fit into consolidated finance views
- follow-up issues for calculations and UI can be derived from the result