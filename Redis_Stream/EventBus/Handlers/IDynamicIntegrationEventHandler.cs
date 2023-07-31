namespace Redis_Stream.EventBus.Handlers
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
