using Redis_Stream.EventBus.Events;

namespace Redis_Stream.EventBus.Manager
{
    public interface IIntegrationEventHandler<T> where T : IntegrationEvent
    {
    }
}