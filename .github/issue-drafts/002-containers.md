# Implement cross-domain containers

## Summary

Introduce user-defined containers that group related data across finance, documents, storage, and contracts.

## Why

Containers are a core differentiator of ocent and enable use cases such as yearly tax organization or event-based case grouping.

## Scope

- create container entity and metadata model
- allow records from different domains to be assigned to a container
- support manual container creation and assignment
- define how one record can belong to one or multiple containers

## Acceptance Criteria

- users can create named containers
- finance and document records can be assigned to a container
- the model allows extension to storage and contracts
- a sample use case such as `Income Tax 2026` is supported