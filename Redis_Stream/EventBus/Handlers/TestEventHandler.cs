namespace Redis_Stream.EventBus.Handlers
{
    public class TestEventHandler : IDynamicIntegrationEventHandler
    {
        public async Task Handle(dynamic eventData)
        {
            //// DO SOMETHING
            await Task.CompletedTask;
            Console.WriteLine("hello");
            var a = eventData;
        }
    }
}
