namespace ocent.Backend.WebApi.Domain.Shared;

public record EntityTag(
  Guid Id,
  Guid TagId,
  string EntityType,
  Guid EntityId,
  Guid TaggedByUserId,
  DateTimeOffset TaggedAt
);
