using Azure;
using Azure.Storage.Queues.Models;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class GetMessagesAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        public static TheoryData<string[]> ValidStrings => new TheoryData<string[]>
        {
            {
                new[] {"1", "2", "3"}
            },
            {
                new[] {"1.1", "2.2", "3.3"}
            },
            {
                new[] {"hi there", "how are", "you today"}
            },
            {
                new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
            },
            {
                new[] { "[1,2,3]", "[4,5,6]", "[7,8,9]" }
            }
        };

        public static TheoryData<object[]> ValidComplexObjects => new TheoryData<object[]>
        {
            {
                new[]
                {
                    new FakeThing
                    {
                        Id = 1,
                        Name = "name",
                        NickNames = new[]
                        {
                            "name-1",
                            "name-2"
                        }
                    },
                    new FakeThing
                    {
                        Id = 2,
                        Name = "name-2",
                        NickNames = new[]
                        {
                            "name-3",
                            "name-4"
                        }
                    }
                }
            },
            {
                new[]
                {
                    Enumerable.Range(1, 5).ToList(),
                    Enumerable.Range(1, 5).ToList()
                }
            }
        };

        [Theory]
        [MemberData(nameof(ValidStrings))]
        public async Task GivenSomeQueueMessagesWithSomeStringContent_GetMessagesAsync_ReturnsMessages(string[] contents)
        {
            // Arrange.
            SetupQueue(contents);

            // Act.
            var result = await Queue.GetMessagesAsync(contents.Length);

            // Assert.
            result.ShouldNotBeNull();
            result.Select(r => r.Model)
                  .SequenceEqual(contents)
                  .ShouldBeTrue();

            QueueClient.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(ValidComplexObjects))]
        public async Task GivenAQueueMessageWithSomeJsonContent_GetMessagesAsync_ReturnsAMessage<T>(T[] someObjects)
        {
            // Arrange.
            SetupQueue(someObjects);
            var expected = someObjects.Select(obj => JsonSerializer.Serialize(obj));

            // Act.
            var result = await Queue.GetMessagesAsync<T>(someObjects.Length);

            // Assert.
            result.ShouldNotBeNull();
            result.Select(r => JsonSerializer.Serialize(r.Model))
                  .SequenceEqual(expected)
                  .ShouldBeTrue();

            QueueClient.VerifyAll();
        }

        private void SetupQueue<T>(T[] someObjectsOrStrings)
        {
            var messages = new List<QueueMessage>();

            foreach (var objectOrString in someObjectsOrStrings)
            {
                messages.Add(CreateMessage(objectOrString));
            }

            var response = new Mock<Response<QueueMessage[]>>();
            response.Setup(x => x.Value)
                    .Returns(messages.ToArray());

            QueueClient.Setup(x => x.ReceiveMessagesAsync(It.IsAny<int>(),
                                                          null,
                                                          It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response.Object);
        }
    }
}
