using Azure;
using Azure.Storage.Queues.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
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
                new object[]
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
            foreach (var content in items)
            {
                var messageContent = Helpers.IsASimpleType(typeof(T))
                    ? content.ToString()
                    : JsonSerializer.Serialize(content);

                QueueClient.Setup(x => x.SendMessageAsync(It.Is<BinaryData>(bd => bd.ToString() == messageContent),
                                                           null,
                                                           null,
                                                           It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Mock<Response<SendReceipt>>().Object);
            }


            await Queue.AddMessagesAsync(items, default);

            // Assert.
            QueueClient.VerifyAll();
        }
    }
}
