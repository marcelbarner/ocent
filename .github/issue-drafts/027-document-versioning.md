# Add document versioning

## Summary

Implement document versioning so later revisions of the same document can be tracked without losing history.

## Why

Contract revisions, updated invoices, and corrected document states require a clean historical chain, not separate unrelated files.

## Scope

- define document version relationships
- distinguish document versions from duplicate documents
- distinguish technical duplicates from likely business duplicates
- define current versus historical document states
- define how versioned documents link to contracts and other records

## Acceptance Criteria

- document versioning requirements are documented
- the distinction between version and duplicate is explicit
- technical and business duplicate distinctions are explicit
- current and historical document states are supported conceptually
- the output supports later implementation for contracts and document history