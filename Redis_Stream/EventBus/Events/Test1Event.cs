namespace Redis_Stream.EventBus.Events
{
    public class Test1Event : IntegrationEvent
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
