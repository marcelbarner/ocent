namespace ocent.Backend.WebApi.Domain.Finance;

public enum TransactionStatus
{
  Raw,
  Categorized,
  Reconciled,
  Disputed,
  Archived,
}
