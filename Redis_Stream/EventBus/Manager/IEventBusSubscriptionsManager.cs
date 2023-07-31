using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Handlers;

namespace Redis_Stream.EventBus.Manager
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
        void RemoveSubscription<T, TH>()
                where TH : IIntegrationEventHandler<T>
                where T : IntegrationEvent;
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
        bool HasSubscriptionsForEvent(string eventName);
        Dictionary<string, List<Type>> GetEventType();
        void Clear();
        //IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        //IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
