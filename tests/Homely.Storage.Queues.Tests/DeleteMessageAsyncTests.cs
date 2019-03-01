using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class DeleteMessageAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        [Fact]
        public async Task GivenSomeMessageData_DeleteMessageAsync_DeletesTheMessage()
        {
            // Arrange.
            const string messageId = "1234";
            const string receiptId = "asdasd";
            
            // Act.
            await Queue.DeleteMessageAsync(messageId, receiptId);

            // Assert.
            CloudQueue.Verify(x => x.DeleteMessageAsync(messageId, receiptId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
