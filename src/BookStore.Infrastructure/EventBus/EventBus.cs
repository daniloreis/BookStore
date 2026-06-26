using BookStore.Application.Interfaces;
using BookStore.Domain.Events;

namespace BookStore.Infrastructure.EventBus;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public async Task Publish(DomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();

        if (!_subscribers.TryGetValue(eventType, out var handlers))
            return;

        foreach (var handler in handlers)
        {
            var methodInfo = handler.Method;
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 1 && parameters[0].ParameterType == eventType)
            {
                var task = handler.DynamicInvoke(domainEvent) as Task;
                if (task != null)
                    await task;
            }
        }
    }

    public void Subscribe<T>(IEventHandler<T> handler) where T : DomainEvent
    {
        var eventType = typeof(T);

        if (!_subscribers.ContainsKey(eventType))
            _subscribers[eventType] = new();

        var handleMethod = handler.GetType()
            .GetMethod("Handle", new[] { typeof(T) });

        if (handleMethod != null)
        {
            var delegateHandler = Delegate.CreateDelegate(
                typeof(Func<T, Task>),
                handler,
                handleMethod
            );
            _subscribers[eventType].Add(delegateHandler);
        }
    }
}
