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
    public class GetMessageAsyncTests : FakeAzureStorageQueueCommonTestSetup
    {
        public static IEnumerable<object[]> ValidStrings => new[]
        {
            new object[]
            {
                "1"
            },
            new object[]
            {
                "1.2"
            },
            new object[]
            {
                "this is some content"
            },
            new object[]
            {
                Guid.NewGuid().ToString()
            },
            new object[]
            {
                "[1,2,3,4,5,6,7,8,9,10]"
            }
        };

        public static IEnumerable<object[]> ValidComplexObjects => new[]
        {
            new object[]
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
                }
            },
            new object[]
            {
                Enumerable.Range(1, 50).ToList()
            }
        };

        [Theory]
        [MemberData(nameof(ValidStrings))]
        public async Task GivenAQueueMessageWithSomeStringContent_GetMessageAsync_ReturnsAMessage(string content)
        {
            // Arrange.
            SetupQueue(content);

            // Act.
            var result = await Queue.GetMessageAsync();

            // Assert.
            result.Model.ShouldBe(content);
            QueueClient.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(ValidComplexObjects))]
        public async Task GivenAQueueMessageWithSomeJsonContent_GetMessageAsync_ReturnsAMessage<T>(T someObject)
        {
            // Arrange.
            SetupQueue(someObject);
            var expected = JsonSerializer.Serialize(someObject);

            // Act.
            var result = await Queue.GetMessageAsync<T>();

            // Assert.
            var actual = JsonSerializer.Serialize(result.Model);
            actual.ShouldBe(expected);
            QueueClient.VerifyAll();
        }

        private void SetupQueue<T>(T someObjectOrString)
        {
            var message = CreateMessage(someObjectOrString);
            var response = new Mock<Response<QueueMessage[]>>();
            response.Setup(x => x.Value)
                    .Returns(new[] { message });

            QueueClient.Setup(x => x.ReceiveMessagesAsync(1,
                                                          null,
                                                          It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response.Object);
        }
    }
}
