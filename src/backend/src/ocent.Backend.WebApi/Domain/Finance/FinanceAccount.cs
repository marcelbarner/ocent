using ocent.Backend.WebApi.Domain.Shared;

namespace ocent.Backend.WebApi.Domain.Finance;

public record FinanceAccount(
  Guid Id,
  Guid HouseholdId,
  Guid CreatedByUserId,
  Visibility Visibility,
  bool WriteRestricted,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  LifecycleStage LifecycleStage,
  Dictionary<string, FieldProvenanceEntry>? FieldProvenance,
  AccountType AccountType,
  string Name,
  string Currency,
  decimal CurrentBalance,
  DateOnly BalanceAsOf,
  string? InstitutionName,
  string? Iban,
  string? AccountNumber,
  string? Notes,
  AccountStatus Status,
  Guid? SourceId,
  Dictionary<string, object?>? Metadata
) : OwnedEntity(Id, HouseholdId, CreatedByUserId, Visibility, WriteRestricted, CreatedAt, UpdatedAt, LifecycleStage, FieldProvenance);
