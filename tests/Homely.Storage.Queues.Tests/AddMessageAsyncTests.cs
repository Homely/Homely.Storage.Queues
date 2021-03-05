using Azure;
using Azure.Storage.Queues.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            },
            new object[]
            {
                Enumerable.Range(1, 50)
            }
        };

        [Theory]
        [MemberData(nameof(AddMessageItems))]
        public async Task GivenSomeObjectContent_AddMessageAsync_AddsItToTheQueue<T>(T content)
        {
            // Arrange.
            var isASimpleType = Helpers.IsASimpleType(typeof(T));

            if (isASimpleType)
            {
                QueueClient.Setup(x => x.SendMessageAsync(content.ToString(),
                                                          null,
                                                          null,
                                                          It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Mock<Response<SendReceipt>>().Object);
            }
            else
            {
                var messageContent = JsonSerializer.Serialize(content);
                QueueClient.Setup(x => x.SendMessageAsync(It.Is<BinaryData>(bd => bd.ToString() == messageContent),
                                                          null,
                                                          null,
                                                          It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Mock<Response<SendReceipt>>().Object);
            }

            // Act.
            await Queue.AddMessageAsync(content, default);

            // Assert.
            QueueClient.VerifyAll();
        }
    }
}
