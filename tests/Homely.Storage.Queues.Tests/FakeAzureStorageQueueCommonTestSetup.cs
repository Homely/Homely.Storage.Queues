using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
