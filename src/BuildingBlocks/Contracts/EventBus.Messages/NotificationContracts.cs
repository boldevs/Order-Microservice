using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;


public record NotificationCreate(Guid Id) : IEvent;

public record NotificationUpdate(Guid Id) : IEvent;

public record NotificationDelete(Guid Id) : IEvent;