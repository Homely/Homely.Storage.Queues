using System.Collections.Generic;

namespace Homely.Storage.Queues.Tests
{
    public class FakeThing
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> NickNames { get; set; }
    }
}
