using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class AddMessageAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        public static IEnumerable<object[]> AddMessageItems => new[]
        {
            new object[]
            {
                69
            },
            new object[]
            {
                69.22F
            },
            new object[]
            {
                "aaaaa"
            },
            new object[]
            {
                new FakeThing
                {
                    Id = 1,
                    Name = "Some name",
                    NickNames = new[]
                    {
                        "a",
                        "b",
                        "c"
                    }
                }
            }
        };

        [Theory]
        [MemberData(nameof(AddMessageItems))]
        public async Task GivenSomeObjectContent_AddMessageAsync_AddsItToTheQueue<T>(T content)
        {
            // Arrange.
            var messageContent = Helpers.IsASimpleType(typeof(T)) 
                ? content.ToString()
                : JsonConvert.SerializeObject(content);

            // Act.
            await Queue.AddMessageAsync(content);

            // Assert.
            CloudQueue.Verify(x => x.AddMessageAsync(It.Is<CloudQueueMessage>(y => y.AsString == messageContent),
                                                     null,
                                                     null,
                                                     null,
                                                     null,
                                                     It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
