using Homely.Testing;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
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
            }
        };

        [Theory]
        [MemberData(nameof(ValidStrings))]
        public async Task GivenAQueueMessageWithSomeStringContent_GetMessageAsync_ReturnsAMessage(string content)
        {
            // Arrange.
            var message = new CloudQueueMessage(content);
            CloudQueue.Setup(x => x.GetMessageAsync(null, null, null, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(message);

            // Act.
            var result = await Queue.GetMessageAsync();

            // Assert.
            result.Model.ShouldBe(content);
            CloudQueue.VerifyAll();
        }

        [Fact]
        public async Task GivenAQueueMessageWithSomeJsonContent_GetMessageAsync_ReturnsAMessage()
        {
            // Arrange.
            var someObject = new FakeThing
            {
                Id = 1,
                Name = "name",
                NickNames = new[]
                {
                    "name-1",
                    "name-2"
                }
            };
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(someObject));

            CloudQueue.Setup(x => x.GetMessageAsync(null, null, null, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(message);

            // Act.
            var result = await Queue.GetMessageAsync<FakeThing>();

            // Assert.
            result.Model.ShouldLookLike(someObject);
            CloudQueue.VerifyAll();
        }
    }
}
