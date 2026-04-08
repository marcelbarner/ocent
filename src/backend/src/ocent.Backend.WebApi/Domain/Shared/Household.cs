namespace ocent.Backend.WebApi.Domain.Shared;

public record Household(
  Guid Id,
  string Name,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt
);
