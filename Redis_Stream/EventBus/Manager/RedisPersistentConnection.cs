using Polly;
using Polly.Retry;
using StackExchange.Redis;
using System.Net.Sockets;

namespace Redis_Stream.EventBus.Manager
{
    public class RedisPersistentConnection : IRedisPersistentConnection
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly int _retryCount;
        public bool Disposed;
        public RedisPersistentConnection(IConnectionMultiplexer connection, int retryCount = 5)
        {
            _connection = connection;
            _retryCount = retryCount;
        }

        public bool IsConnected => _connection is { IsConnected: true } && !Disposed;

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public bool TryConnect()
        {
            var policy = RetryPolicy.Handle<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    //_logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s", $"{time.TotalSeconds:n1}");
                }
            );
            policy.Execute(() =>
            {
               /// _connection = ConnectionMultiplexer.Connect("localhost");
               // TODO try to connect redis
            });
            if (IsConnected)
                return true;
            else
            {
                return false;
            }
        }
    }
}
