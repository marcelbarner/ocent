# Add contract containers for long-lived subjects

## Summary

Introduce contract containers that group multiple provider contracts and contract revisions under one long-lived real-world subject.

## Why

Users often think in terms such as `electricity for my apartment`, not in terms of isolated provider contracts. The software should preserve this continuity across provider changes.

## Scope

- model a contract container for one enduring subject such as apartment electricity
- allow multiple provider contracts to belong to one container over time
- allow contract versions or revisions to remain nested under the correct provider contract
- define how transactions and documents can be viewed at both contract and container level
- define how cancellation deadlines are viewed across the container timeline
- define how dashboards show history across provider changes

## Acceptance Criteria

- a long-lived contract container can represent one real-world subject
- multiple contracts can belong to the same container over time
- contract versions remain distinguishable from provider changes
- transactions and documents can be analyzed at container level
- cancellation-related views work across the container timeline
- the output supports historical comparison across providers