# Link contracts with transactions and documents

## Summary

Allow contracts to be linked with related transactions and one or more related documents.

## Why

Contracts should act as the expected baseline for recurring costs and obligations, while transactions represent actual financial movements and documents provide evidence.

## Scope

- link contracts to one or more transactions
- link contracts to one or more documents
- define whether transaction splits can link to contracts directly
- define how links behave across contract versions or revisions
- define how links behave when multiple contracts belong to one contract container
- define how contract-linked transactions support fixed-cost analysis and reconciliation

## Acceptance Criteria

- contracts can be linked to multiple documents
- contracts can be linked to transactions
- the rules for contract links on transaction splits are documented
- contract link behavior across versions is documented
- contract link behavior inside a contract container is documented
- the output supports later fixed-cost and variance analysis