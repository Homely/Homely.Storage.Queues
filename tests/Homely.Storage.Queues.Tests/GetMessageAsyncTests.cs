using Homely.Testing;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var message = CreateMessage(content);
            CloudQueue.Setup(x => x.GetMessageAsync(null, null, null, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(message);

            // Act.
            var result = await Queue.GetMessageAsync();

            // Assert.
            result.Model.ShouldBe(content);
            CloudQueue.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(ValidComplexObjects))]
        public async Task GivenAQueueMessageWithSomeJsonContent_GetMessageAsync_ReturnsAMessage<T>(T someObject)
        {
            // Arrange.
            var message = CreateMessage(someObject);
            CloudQueue.Setup(x => x.GetMessageAsync(null, null, null, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(message);

            // Act.
            var result = await Queue.GetMessageAsync<T>();

            // Assert.
            result.Model.ShouldLookLike(someObject);
            CloudQueue.VerifyAll();
        }
    }
}
