using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace Homely.Storage.Queues.Tests
{
    public class FakeAzureStorageQueue : AzureStorageQueue
    {
        public FakeAzureStorageQueue(CloudQueue cloudQueue,
                                     ILogger<FakeAzureStorageQueue> logger) : base("ignored-connection-string", "ignored-name", logger)
        {
            Queue = Task.FromResult(cloudQueue);
        }
    }
}
