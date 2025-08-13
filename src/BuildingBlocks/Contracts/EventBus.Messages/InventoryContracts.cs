using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record InventoryCreate(Guid Id) : IEvent;

public record InventoryUpdate(Guid Id) : IEvent;

public record InventoryDelete(Guid Id) : IEvent;

public record InventoryStatusUpdated(Guid Id) : IEvent;