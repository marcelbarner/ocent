# Define core domain model and shared entities

## Summary

Create the foundational domain model for ocent so that finance, documents, storage, contracts, containers, and pipelines can evolve on top of a consistent structure.

## Why

ocent depends on strong cross-domain relationships. Without a clear shared model, later linking and analytics will become inconsistent.

## Scope

- define primary entities for each domain
- define shared concepts such as user, container, tag, link, source, and status
- define stable identifiers and ownership boundaries
- define lifecycle states for records that start raw and are enriched later
- define field-level provenance concepts such as manual, imported, OCR-extracted, and AI-proposed
- define household-aware ownership and visibility concepts

## Acceptance Criteria

- a written domain model exists for the initial product scope
- shared entity relationships are explicitly documented
- cross-domain link rules are defined
- provenance and ownership concepts are explicitly documented
- open modeling decisions are captured for follow-up issues