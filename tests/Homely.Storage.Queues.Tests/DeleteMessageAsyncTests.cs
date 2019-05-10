using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class DeleteMessageAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        [Fact]
        public async Task GivenSomeGenericMessageData_DeleteMessageAsync_DeletesTheMessage()
        {
            // Arrange.
            const string content = "aaaaa";
            const string id = "1234";
            const string receiptId = "asdasd";
            const int dequeueCount = 5;
            var message = new Message(content, id, receiptId, dequeueCount);

            // Act.
            await Queue.DeleteMessageAsync(message);

            // Assert.
            CloudQueue.Verify(x => x.DeleteMessageAsync(id, receiptId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GivenSomeSpecificMessageData_DeleteMessageAsync_DeletesTheMessage()
        {
            // Arrange.
            var fakeThing = new FakeThing
            {
                Id = 1,
                Name = "name"
            };
            const string id = "1234";
            const string receiptId = "asdasd";
            const int dequeueCount = 5;
            var message = new Message<FakeThing>(fakeThing, id, receiptId, dequeueCount);

            // Act.
            await Queue.DeleteMessageAsync(message);

            // Assert.
            CloudQueue.Verify(x => x.DeleteMessageAsync(id, receiptId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}