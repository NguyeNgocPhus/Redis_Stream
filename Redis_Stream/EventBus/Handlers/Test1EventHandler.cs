namespace Redis_Stream.EventBus.Handlers
{
    public class Test1EventHandler : IDynamicIntegrationEventHandler
    {
        public async Task Handle(dynamic eventData)
        {
            //// DO SOMETHING
            await Task.CompletedTask;
            Console.WriteLine("Hi");
            var a = eventData;
        }
    }
}
