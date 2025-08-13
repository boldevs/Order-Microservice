using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record ShipCreated(Guid Id) : IEvent;

public record ShipUpdated(Guid Id) : IEvent;

public record ShipDelete(Guid Id) : IEvent;

public record ShipStatusUpdated(Guid Id, string Status) : IEvent;