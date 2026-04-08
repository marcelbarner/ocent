# Add portfolio and depot tracking

## Summary

Implement investment portfolio tracking for depots and securities events.

## Why

Portfolio events are part of the financial reality of private users and should be represented alongside accounts and documents.

## Scope

- model depots or portfolios
- capture buys
- capture sells
- capture dividends
- define portfolio KPI requirements such as XIRR, TTWROR, absolute delta, relative delta, and dividend yield
- define projected capital income requirements such as future dividends and interest
- prepare event records for linking with source documents and containers

## Acceptance Criteria

- users can create a depot or portfolio
- users can record buy, sell, and dividend events
- events can be linked to related documents
- KPI requirements for depot and portfolio analysis are documented for implementation
- projected capital income requirements are documented for retirement-oriented dashboards
- events are compatible with future reporting and analytics