namespace ocent.Backend.WebApi.Domain.Shared;

public record ContainerMembership(
  Guid Id,
  Guid ContainerId,
  string EntityType,
  Guid EntityId,
  Guid AssignedByUserId,
  DateTimeOffset AssignedAt,
  string? Note
);
