# Add contract versioning and condition history

## Summary

Introduce contract versions or revisions so price and term changes are stored historically instead of overwriting the current contract state.

## Why

Users need to understand how contract conditions evolved over time, for example when an electricity tariff changes its working price or monthly base price.

## Scope

- model contract versions or revisions
- support condition phases with validity ranges
- represent changing values such as working price, monthly base price, and other terms
- define the relationship between contract versions and higher-level contract containers
- model rule changes for cancellation periods across contract phases
- define how current and historical versions are shown in dashboards
- define how version history supports fixed-cost analysis and transaction reconciliation

## Acceptance Criteria

- contract history can represent multiple condition phases over time
- changing prices and terms are stored historically
- current and historical contract states are distinguishable
- contract versions fit into a long-lived contract container model
- cancellation period changes across phases are represented in the model
- dashboard requirements for condition history are documented
- the output supports later implementation for utilities and similar recurring contracts