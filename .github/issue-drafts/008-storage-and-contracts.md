# Define storage, catalog, and contracts domain foundations

## Summary

Define and implement the initial structure for the storage, catalog, and contracts domains.

## Why

Storage, catalog, and contracts are part of the intended product scope, but their detailed requirements are still less mature than finance and documents.

## Scope

- define first-pass goals for storage, catalog, and contracts
- identify the minimum entity set for each domain
- align these domains with containers, documents, and future analytics
- define how product hierarchy and storage location hierarchy fit the shared model
- define how contracts link to transactions and documents
- define how contract versions and price phases are represented historically
- define how contract containers group multiple provider contracts into one long-lived subject
- capture open questions that need product decisions

## Acceptance Criteria

- storage domain basics are documented
- catalog domain basics are documented
- contracts domain basics are documented
- contract links to transactions and documents are defined
- contract versioning and historical condition modeling are defined
- contract container modeling is defined
- both domains fit into the shared cross-domain model
- follow-up implementation issues are identified