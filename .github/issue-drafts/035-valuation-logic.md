# Add valuation logic for non-trivial assets and obligations

## Summary

Implement explicit valuation logic for assets and obligations that are not simple balance-based records.

## Why

Net worth views become misleading if real estate, pension values, receivables, or illiquid objects lack a clear valuation approach.

## Scope

- define valuation approaches for real estate
- define valuation approaches for pension-related values
- define valuation approaches for receivables and similar claims
- define how uncertain or estimated values are represented
- define how valuation choices affect analytics and dashboards

## Acceptance Criteria

- valuation approaches are documented for the main non-trivial finance objects
- uncertainty or estimate handling is explicit
- dashboard and analytics consequences are described conceptually
- the output supports later implementation for net worth calculations