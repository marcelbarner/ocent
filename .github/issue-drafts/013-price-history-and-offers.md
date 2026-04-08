# Track price history and offers by product and merchant

## Summary

Implement merchant-aware price tracking so users can analyze price development and identify attractive offers.

## Why

The catalog system should later support smarter purchasing decisions, not only inventory tracking.

## Scope

- store price observations from receipts, invoices, or manual entries
- relate prices to products and merchants
- support historical price development per product
- support merchant comparison for the same product or product family
- define rules for highlighting notable offers

## Acceptance Criteria

- price records can be linked to products and merchants
- price history can be queried over time
- merchant-specific comparisons are supported in the model
- offer detection requirements are documented for dashboards and alerts