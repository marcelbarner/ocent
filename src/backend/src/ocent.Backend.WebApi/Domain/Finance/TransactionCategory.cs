namespace ocent.Backend.WebApi.Domain.Finance;

public record TransactionCategory(
  Guid Id,
  Guid HouseholdId,
  Guid? ParentId,
  string Name,
  bool IsIncome,
  bool IsTaxRelevant,
  string? ColorHex,
  DateTimeOffset CreatedAt
);
