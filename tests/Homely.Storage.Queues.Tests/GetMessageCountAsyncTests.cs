using Azure;
using Azure.Storage.Queues.Models;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class GetMessageCountAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        [Fact]
        public async Task GivenAQueue_GetMessageCountAsync_ReturnsMessageCount()
        {
            // Arrange.
            var messageCount = 100;
            var queueProperties = QueuesModelFactory.QueueProperties(new Dictionary<string, string>(), messageCount);
            var queuePropertiesResponse = new Mock<Response<QueueProperties>>(MockBehavior.Strict);
            queuePropertiesResponse.Setup(x => x.Value)
                                   .Returns(queueProperties);

            QueueClient.Setup(x => x.GetPropertiesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(queuePropertiesResponse.Object);

            // Act.
            var actualMessageCount = await Queue.GetMessageCountAsync();

            // Assert.
            actualMessageCount.ShouldBe(messageCount);

            QueueClient.VerifyAll();
        }
    }
}
