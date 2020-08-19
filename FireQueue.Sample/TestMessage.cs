using FireQueue.Core.Infrastructure;
using System;

namespace FireQueue.Sample
{
    class TestMessage : IMessage
    {
        public string TestString { get; set; }
        public int TestInt { get; set; }
        public DateTime DatePublished { get; set; }
    }
}
