using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text.Json;

namespace Homely.Storage.Queues.Tests
{
    public abstract class FakeAzureStorageQueueCommonTestSetup
    {
        protected FakeAzureStorageQueueCommonTestSetup()
        {
            QueueClient = new Mock<QueueClient>(MockBehavior.Strict);
            Logger = new Mock<ILogger<FakeAzureStorageQueue>>();
            Queue = new FakeAzureStorageQueue(QueueClient.Object, Logger.Object);
        }

        protected Mock<QueueClient> QueueClient { get; }
        protected Mock<ILogger<FakeAzureStorageQueue>> Logger { get; }
        protected FakeAzureStorageQueue Queue { get; }

        protected QueueMessage CreateMessage<T>(T someObject)
        {
            const string id = "aaa";
            const string popReceipt = "bbb";

            var messageText = Helpers.IsASimpleType(typeof(T))
                    ? someObject.ToString()
                    : JsonSerializer.Serialize(someObject);

            return QueuesModelFactory.QueueMessage(id,
                                                   popReceipt,
                                                   Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageText)),
                                                   0);
        }
    }
}
