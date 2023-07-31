using Hangfire;
using Microsoft.VisualBasic;

namespace Redis_Stream
{
    public interface IWorkerService
    {
        public Task SyncDataFromDmpAsync(CancellationToken cancellationToken);

        [Queue("ok")]
        public Task Start(CancellationToken cancellationToken);
    }

}
