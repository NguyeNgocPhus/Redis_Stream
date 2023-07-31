using StackExchange.Redis;

namespace Redis_Stream.EventBus.Manager
{
    public interface IRedisPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IDatabase GetDatabase();
    }
}
