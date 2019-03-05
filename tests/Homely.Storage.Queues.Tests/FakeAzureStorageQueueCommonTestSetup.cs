using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using System;

namespace Homely.Storage.Queues.Tests
{
    public abstract class FakeAzureStorageQueueCommonTestSetup
    {
        protected FakeAzureStorageQueueCommonTestSetup()
        {
            CloudQueue = new Mock<CloudQueue>(new Uri("http://a.b.c.d"));
            Logger = new Mock<ILogger<FakeAzureStorageQueue>>();
            Queue = new FakeAzureStorageQueue(CloudQueue.Object, Logger.Object);
        }

        protected Mock<CloudQueue> CloudQueue { get; }
        protected Mock<ILogger<FakeAzureStorageQueue>> Logger { get; }
        protected FakeAzureStorageQueue Queue { get; }

        protected CloudQueueMessage CreateMessage<T>(T someObject)
        {
            const string id = "aaa";
            const string popReceipt = "bbb";

            var content = Helpers.IsASimpleType(typeof(T))
                    ? someObject.ToString()
                    : JsonConvert.SerializeObject(someObject);

            var message = new CloudQueueMessage(id, popReceipt);
            message.SetMessageContent2(System.Text.Encoding.UTF8.GetBytes(content));

            return message;
        }
    }
}
