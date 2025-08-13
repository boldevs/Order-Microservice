using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;


public record AccountCreate(Guid Id) : IEvent;

public record AccountUpdate(Guid Id) : IEvent;

public record AccountDelete(Guid Id) : IEvent;

public record AccountStatusUpdate(Guid Id) : IEvent;
