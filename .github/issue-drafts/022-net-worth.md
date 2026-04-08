# Add net worth and wealth development views

## Summary

Implement a net worth view that combines major assets and liabilities into one historical wealth perspective.

## Why

The finance domain now covers accounts, depots, pension insurance, real estate, receivables, and liabilities. These need a consolidated wealth view instead of remaining isolated.

## Scope

- define how money accounts, depots, pension insurance, real estate, receivables, and liabilities contribute to net worth
- define current net worth calculation rules
- define historical net worth development over time
- define how different valuation sources are represented where needed
- define valuation logic for assets and obligations that require estimates or non-balance-based values

## Acceptance Criteria

- net worth inputs are documented across the relevant finance entities
- current and historical net worth views are defined
- valuation questions are identified where needed
- valuation rules are explicit enough to support later implementation
- the output supports later dashboard and analytics implementation