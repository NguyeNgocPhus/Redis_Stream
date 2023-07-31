using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Handlers;

namespace Redis_Stream.EventBus.Services
{
    public interface IEventBus
    {
        public Task Subscribe<TH>(string streamName, string comsumerGroupName, List<string> comsumers) where TH : IDynamicIntegrationEventHandler;
        public Task Publish(IntegrationEvent @event);
    }
}
