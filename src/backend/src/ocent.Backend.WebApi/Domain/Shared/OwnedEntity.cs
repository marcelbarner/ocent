namespace ocent.Backend.WebApi.Domain.Shared;

public abstract record OwnedEntity(
  Guid Id,
  Guid HouseholdId,
  Guid CreatedByUserId,
  Visibility Visibility,
  bool WriteRestricted,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  LifecycleStage LifecycleStage,
  Dictionary<string, FieldProvenanceEntry>? FieldProvenance
);
