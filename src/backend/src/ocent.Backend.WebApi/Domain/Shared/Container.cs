namespace ocent.Backend.WebApi.Domain.Shared;

public record Container(
  Guid Id,
  Guid HouseholdId,
  Guid CreatedByUserId,
  Visibility Visibility,
  bool WriteRestricted,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  LifecycleStage LifecycleStage,
  Dictionary<string, FieldProvenanceEntry>? FieldProvenance,
  string Name,
  string? Description,
  ContainerKind Kind,
  int? TaxYear,
  ContainerStatus Status,
  Dictionary<string, object?>? Metadata
) : OwnedEntity(Id, HouseholdId, CreatedByUserId, Visibility, WriteRestricted, CreatedAt, UpdatedAt, LifecycleStage, FieldProvenance);
