# Add upload validation and duplicate handling

## Summary

Implement explicit upload-time validation and duplicate handling for document intake, while keeping heavier processing asynchronous.

## Why

Document uploads should feel immediate and safe, while still preventing obvious duplicate files and preserving a clean review flow for more ambiguous duplicate situations.

## Scope

- define synchronous upload validation checks such as readability, allowed type, and technical integrity
- define exact or near-exact technical duplicate detection at intake time
- define likely business duplicate detection for later review
- define which document processing steps remain asynchronous after upload
- define status visibility for upload, background processing, and review needs

## Acceptance Criteria

- upload-time validation responsibilities are documented
- technical duplicate handling rules are explicit
- business duplicate review rules are explicit
- the boundary between synchronous intake and asynchronous processing is documented
- the output supports later implementation for document intake and pipelines