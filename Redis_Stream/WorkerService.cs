using Hangfire;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Manager;
using System.Runtime.CompilerServices;
using static Dapper.SqlMapper;

namespace Redis_Stream
{
    public class WorkerService : IWorkerService
    {
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IRedisPersistentConnection _connection;

        public WorkerService(IEventBusSubscriptionsManager subsManager, IRedisPersistentConnection connection)
        {
            _subsManager = subsManager;
            _connection = connection;
        }

        public async Task SyncDataFromDmpAsync(CancellationToken cancellationToken)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }
            var db = _connection.GetDatabase();

            //await Task.CompletedTask;
            var handlers = _subsManager.GetEventType();
            foreach (var handler in handlers)
            {
                var key = handler.Key.Split("-");
                var streamName = key[0];
                var groupName = key[1];
                var typeHandlers = handler.Value;

                var result = await db.StreamReadGroupAsync(streamName, groupName, "A", ">", null);
                if (result == null || result.Count() == 0) 
                    continue;

                foreach (var type in typeHandlers)
                {
                    object instance = RuntimeHelpers.GetUninitializedObject(type);
                    var x = instance.GetType().GetMethod("Handle", new Type[1]
                    {
                         typeof(object)
                    }, null);
                    x.Invoke(instance, new object?[1]
                    {
                        "data"
                    });

                }
            }


        }
        public async Task Start(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            RecurringJob.AddOrUpdate<IWorkerService>(
             "redis-stream",
             w => w.SyncDataFromDmpAsync(CancellationToken.None), "*/15 * * * * *");

        }

    }
}
