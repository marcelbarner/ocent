# Add liquidity forecasting

## Summary

Implement forward-looking liquidity forecasts based on expected inflows and outflows.

## Why

Users need to understand not only what they own overall, but also whether enough available cash will remain in the near future.

## Scope

- define which expected inflows and outflows participate in liquidity forecasts
- include recurring transactions, contract-driven costs, and planned payments where applicable
- define how budgeting inputs influence liquidity forecasts where applicable
- define forecast horizons such as upcoming days, weeks, and months
- define warning behavior for projected shortfalls

## Acceptance Criteria

- liquidity forecast inputs are documented
- forecast horizons are defined
- the relationship between budgeting and liquidity forecasts is documented
- projected shortfall logic is described conceptually
- the output supports later finance dashboard implementation