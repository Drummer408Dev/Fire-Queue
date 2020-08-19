using FireQueue.Core.Infrastructure;
using System;

namespace FireQueue.Sample
{
    class AnotherTestMessageHandler : MessageHandler<TestMessage>
    {
        public override void Handle(TestMessage message)
        {
            Console.WriteLine($"From AnotherTestMessageHandler: {message.TestString}");
        }
    }
}
