namespace ocent.Backend.WebApi.Domain.Shared;

public record Tag(
  Guid Id,
  Guid HouseholdId,
  string Name,
  string? ColorHex,
  DateTimeOffset CreatedAt
);
