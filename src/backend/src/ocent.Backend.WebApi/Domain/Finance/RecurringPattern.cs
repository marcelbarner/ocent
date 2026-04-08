namespace ocent.Backend.WebApi.Domain.Finance;

public record RecurringPattern(
  Guid Id,
  Guid HouseholdId,
  Guid CreatedByUserId,
  Guid FinanceAccountId,
  string Name,
  decimal? ExpectedAmount,
  string Currency,
  RecurringFrequency Frequency,
  int? DayOfMonth,
  Guid? CategoryId,
  Guid? LinkedContractVersionId,
  DateOnly ActiveFrom,
  DateOnly? ActiveTo
)
{
  public bool IsActive { get; init; } = true;
}
