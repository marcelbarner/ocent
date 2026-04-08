# Add import channels beyond CSV

## Summary

Implement additional import channels beyond CSV, including bulk document intake and other structured entry points.

## Why

Long-term central usage requires more than manual entry and CSV-based imports.

## Scope

- define bulk document import flows
- define drag-and-drop collection imports
- define email-based intake where appropriate
- define additional finance import formats where applicable
- define how these imports interact with pipelines and review flows
- define upload-time validation and duplicate handling across import channels

## Acceptance Criteria

- non-CSV import channels are documented
- their relation to review and pipeline flows is explicit
- document and finance intake expectations are covered
- validation and duplicate handling expectations across channels are explicit
- the output supports later implementation for broader ingestion