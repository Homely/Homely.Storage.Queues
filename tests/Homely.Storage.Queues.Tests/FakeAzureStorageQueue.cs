using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Homely.Storage.Queues.Tests
{
    public class FakeAzureStorageQueue : AzureQueue
    {
        public FakeAzureStorageQueue(QueueClient cloudQueue,
                                     ILogger<FakeAzureStorageQueue> logger) : base("ignored-connection-string", "ignored-name", logger)
        {
            Queue = Task.FromResult(cloudQueue);
        }
    }
}
