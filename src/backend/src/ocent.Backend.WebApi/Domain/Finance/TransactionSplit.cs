using ocent.Backend.WebApi.Domain.Shared;

namespace ocent.Backend.WebApi.Domain.Finance;

public record TransactionSplit(
  Guid Id,
  Guid HouseholdId,
  Guid TransactionId,
  decimal Amount,
  string Currency,
  Guid? CategoryId,
  string? Description,
  string? Notes,
  Dictionary<string, FieldProvenanceEntry>? FieldProvenance
);
