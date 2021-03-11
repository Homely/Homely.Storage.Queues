using System;
using System.Text;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Homely.Storage.Queues.Tests
{
    public class AsStringTests
    {
        private static FakeThing _fakeThing = new()
        {
            Id = 1,
            Name = "Joe Bloggs",
            NickNames = new[] { "joe", "bloggsy" }
        };

        public static TheoryData<BinaryData, string> Data => new TheoryData<BinaryData, string>
        {
            // string
            {
                BinaryData.FromString(Convert.ToBase64String(Encoding.UTF8.GetBytes(("AAAA")))),
                "AAAA"
            },

            // Complex object
            {
                BinaryData.FromString(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_fakeThing)))),
                "{\"Id\":1,\"Name\":\"Joe Bloggs\",\"NickNames\":[\"joe\",\"bloggsy\"]}"
            },
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void GivenSomeBinaryData_AsString_ReturnsString(BinaryData binaryData, string expectedString)
        {
            binaryData.AsString()
                      .ShouldBe(expectedString);
        }
    }
}
