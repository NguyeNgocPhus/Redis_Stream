using Polly;
using Polly.Retry;
using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Handlers;
using Redis_Stream.EventBus.Manager;
using StackExchange.Redis;
using System;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Xml.Linq;

namespace Redis_Stream.EventBus.Services
{
    public class EventBus : IEventBus
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IRedisPersistentConnection _connection;
        private readonly int _retryCount = 5;
        public EventBus(IConnectionMultiplexer redis, IEventBusSubscriptionsManager subsManager, IRedisPersistentConnection connection)
        {
            Console.WriteLine("okkk");
            _redis = redis;
            _subsManager = subsManager;
            _connection = connection;
        }



        public async Task Publish(IntegrationEvent @event)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
                // try to connect
            }
            var policy = RetryPolicy.Handle<Exception>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                //_logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s", @event.Id, $"{time.TotalSeconds:n1}");
            });
            var streamName = @event.GetType().Name;
            var entities = new NameValueEntry[] { new NameValueEntry("Name","phunn") { } };
            var db = _connection.GetDatabase();

            await policy.Execute(async () =>
            {
                // add log
                await db.StreamAddAsync(streamName, entities);
            });
        }

        public async Task Subscribe<TH>(string streamName, string comsumerGroupName, List<string> comsumers) where TH : IDynamicIntegrationEventHandler
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
                // try to connect
            }
            var db = _connection.GetDatabase();

            var policy = RetryPolicy.Handle<Exception>()
             .Or<SocketException>()
             .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
             {
                 //_logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s", @event.Id, $"{time.TotalSeconds:n1}");
             });
            await policy.Execute(async () =>
            {
                // add log
                if (!(await db.KeyExistsAsync(streamName)) ||
                 (await db.StreamGroupInfoAsync(streamName)).All(x => x.Name != comsumerGroupName))
                {
                    await db.StreamCreateConsumerGroupAsync(streamName, comsumerGroupName, "0-0", true);
                }
            });
            var a = typeof(IDynamicIntegrationEventHandler);
            _subsManager.AddSubscription<TH>(eventName: $"{streamName}-{comsumerGroupName}");
            var dd = 1;
        }
    }
}
