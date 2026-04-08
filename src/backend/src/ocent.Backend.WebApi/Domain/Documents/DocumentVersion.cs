namespace ocent.Backend.WebApi.Domain.Documents;

public record DocumentVersion(
  Guid Id,
  Guid DocumentId,
  int VersionNumber,
  string? ChangeDescription,
  Guid CreatedByUserId,
  DateTimeOffset CreatedAt
);
