using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Handlers;

namespace Redis_Stream.EventBus.Manager
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<Type>> _handlers = new ();

        public event EventHandler<string> OnEventRemoved;

        public bool IsEmpty => _handlers is { Count: 0 };
        public void Clear() => _handlers.Clear();
        public void AddSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }
            if (_handlers[eventName].Any(s => s == typeof(TH)))
            {
                throw new ArgumentException(
                    $"Handler Type {typeof(TH).Name} already registered for '{eventName}'");
            }
            _handlers[eventName].Add(typeof(TH));
        }
        public string GetEventKey<T>()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<Type>> GetEventType()
        {
            return _handlers;
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
