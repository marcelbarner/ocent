namespace ocent.Backend.WebApi.Domain.Shared;

public record User(
  Guid Id,
  Guid HouseholdId,
  string Email,
  string DisplayName,
  string PasswordHash,
  string? TotpSecret,
  UserRole Role,
  Visibility VisibilityDefault,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  DateTimeOffset? LastLoginAt,
  DateTimeOffset? ArchivedAt
);
