# Build catalog and inventory system

## Summary

Implement a catalog and inventory system for household goods and consumables with product hierarchy, quantity tracking, and automated updates from receipts and invoices.

## Why

The storage domain should not only represent places but also product knowledge, current quantities, and practical household availability.

## Scope

- model canonical products with aliases and OCR variants
- support hierarchical product structures such as category, brand, and specific product
- track current quantity per product
- support quantity aggregation across hierarchy levels
- update quantities from receipts or invoices where possible
- prepare the model for later recipe and meal matching

## Acceptance Criteria

- products can be modeled in a hierarchy
- quantities can be tracked on concrete products
- higher hierarchy levels can expose aggregated quantities
- receipt or invoice processing can propose or apply quantity changes
- the output supports later recipe and offer use cases