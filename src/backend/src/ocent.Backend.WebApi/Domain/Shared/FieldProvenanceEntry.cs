namespace ocent.Backend.WebApi.Domain.Shared;

public record FieldProvenanceEntry(
  ProvenanceSource Source,
  double? Confidence,
  bool Reviewed,
  Guid? ReviewedByUserId,
  DateTimeOffset? ReviewedAt
);
