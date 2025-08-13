using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record OrderCreated(Guid Id) : IIntegrationEvent;

public record OrderUpdated(Guid Id) : IIntegrationEvent;

public record OrderDeleted(Guid Id) : IIntegrationEvent;

public record OrderItemAdded(Guid OrderItemId) : IIntegrationEvent;

public record OrderItemUpdated(Guid OrderItemId) : IIntegrationEvent;

public record OrderItemDeleted(Guid OrderItemId) : IIntegrationEvent;


