# Add field-level data provenance and auditability

## Summary

Implement field-level provenance so users can see how important values entered the system and whether they were verified.

## Why

ocent relies on manual entry, imports, OCR, and AI suggestions. Without provenance, trust and reviewability degrade quickly.

## Scope

- define provenance states such as manual, imported, OCR-extracted, AI-proposed, and derived
- define verification or review states where appropriate
- define how provenance appears in UI and audit views
- define which fields require provenance tracking

## Acceptance Criteria

- provenance states are documented
- reviewability expectations are explicit
- UI or audit surfacing is defined conceptually
- the result supports later implementation across domains