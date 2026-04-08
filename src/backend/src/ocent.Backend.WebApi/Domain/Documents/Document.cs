using ocent.Backend.WebApi.Domain.Shared;

namespace ocent.Backend.WebApi.Domain.Documents;

public record Document(
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
  string DocumentType,
  DateOnly? IssueDate,
  string? Issuer,
  DocumentStatus Status,
  Guid? SourceId
) : OwnedEntity(Id, HouseholdId, CreatedByUserId, Visibility, WriteRestricted, CreatedAt, UpdatedAt, LifecycleStage, FieldProvenance);
