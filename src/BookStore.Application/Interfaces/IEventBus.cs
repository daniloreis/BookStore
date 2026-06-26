using BookStore.Domain.Events;

namespace BookStore.Application.Interfaces;

public interface IEventBus
{
    Task Publish(DomainEvent domainEvent);
    void Subscribe<T>(IEventHandler<T> handler) where T : DomainEvent;
}

public interface IEventHandler<T> where T : DomainEvent
{
    Task Handle(T @event);
}
