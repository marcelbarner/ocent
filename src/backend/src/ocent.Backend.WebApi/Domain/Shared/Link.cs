namespace ocent.Backend.WebApi.Domain.Shared;

public record Link(
  Guid Id,
  Guid HouseholdId,
  string SourceEntityType,
  Guid SourceEntityId,
  string TargetEntityType,
  Guid TargetEntityId,
  string LinkType,
  LinkDirection Direction,
  Guid CreatedByUserId,
  DateTimeOffset CreatedAt,
  string? Note,
  ProvenanceSource ProvenanceSource
);
