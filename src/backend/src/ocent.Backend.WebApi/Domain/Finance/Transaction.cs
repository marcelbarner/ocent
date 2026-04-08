using ocent.Backend.WebApi.Domain.Shared;

namespace ocent.Backend.WebApi.Domain.Finance;

public record Transaction(
  Guid Id,
  Guid HouseholdId,
  Guid CreatedByUserId,
  Visibility Visibility,
  bool WriteRestricted,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  LifecycleStage LifecycleStage,
  Dictionary<string, FieldProvenanceEntry>? FieldProvenance,
  Guid AccountId,
  DateOnly BookingDate,
  DateOnly? ValueDate,
  decimal Amount,
  string Currency,
  string? Description,
  string? ExternalId,
  string? CounterpartName,
  string? CounterpartIban,
  Guid? CategoryId,
  bool IsSplit,
  Guid? RecurringPatternId,
  TransactionStatus Status,
  Guid? SourceId,
  string? Notes
) : OwnedEntity(Id, HouseholdId, CreatedByUserId, Visibility, WriteRestricted, CreatedAt, UpdatedAt, LifecycleStage, FieldProvenance);
