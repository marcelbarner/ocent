namespace ocent.Backend.WebApi.Domain.Documents;

public record DocumentFile(
  Guid Id,
  Guid DocumentVersionId,
  string StoragePath,
  string MimeType,
  long FileSizeBytes,
  string ChecksumSha256,
  DateTimeOffset CreatedAt
);
