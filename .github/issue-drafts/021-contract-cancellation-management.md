# Add contract cancellation deadline management

## Summary

Implement contract cancellation logic including visible cancellation periods, next possible cancellation dates, rule-based deadline changes, and reminders.

## Why

Users need to act on contracts in time, especially when cancellation rules change after a minimum contract term or due to other legal or contractual transitions.

## Scope

- store the currently effective cancellation period
- calculate the next possible cancellation date
- support rule changes after defined contract phases
- support reminders and warnings ahead of important cancellation dates
- define how these rules are shown in contract dashboards and timelines

## Acceptance Criteria

- cancellation periods can be represented explicitly
- next possible cancellation dates can be derived
- rule-based deadline changes are supported conceptually
- reminder and warning requirements are documented
- the output supports implementation for utilities, telecom, and similar contracts