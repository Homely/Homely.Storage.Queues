using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class AddMessagesAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        public static IEnumerable<object[]> AddMessagesItems => new[]
        {
            new object[]
            {
                new object[]
                {
                    69,
                    1,
                    2,
                    3,
                    4
                }
            },

            new object[]
            {
                new []
                {
                    "aaaa",
                    "bbbbb",
                    "ccccc"
                }
            },

            new object[]
            {
                new[]
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
                    },
                    new FakeThing(),
                    new FakeThing
                    {
                        Id = 1,
                    }
                }
            },

            new object[]
            {
                new object[]
                {
                    69,
                    1.1,
                    22.3,
                    "aaaa",
                    new FakeThing
                    {
                        Id = 1234
                    }
                }
            },
        };

        [Theory]
        [MemberData(nameof(AddMessagesItems))]
        public async Task GivenSomeObjectContents_AddMessagesAsync_AddsItToTheQueue<T>(IEnumerable<T> items)
        {
            // Arrange & Act.
            await Queue.AddMessagesAsync(items, default);

            // Assert.
            foreach (var content in items)
            {
                var messageContent = Helpers.IsASimpleType(typeof(T))
                    ? content.ToString()
                    : JsonConvert.SerializeObject(content);

                CloudQueue.Verify(x => x.AddMessageAsync(It.Is<CloudQueueMessage>(y => y.AsString == messageContent),
                                                         null,
                                                         null,
                                                         null,
                                                         null,
                                                         It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
