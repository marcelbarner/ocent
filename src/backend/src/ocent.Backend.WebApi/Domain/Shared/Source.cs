namespace ocent.Backend.WebApi.Domain.Shared;

public record Source(
  Guid Id,
  Guid HouseholdId,
  SourceKind Kind,
  string Label,
  DateTimeOffset ImportedAt,
  Guid ImportedByUserId,
  string? FileReference,
  Dictionary<string, object?>? Metadata
);
