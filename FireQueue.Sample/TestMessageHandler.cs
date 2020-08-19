using FireQueue.Core.Infrastructure;
using System;

namespace FireQueue.Sample
{
    class TestMessageHandler : MessageHandler<TestMessage>
    {
        public override void Handle(TestMessage message)
        {
            Console.WriteLine("I HAVE BEEN HANDLED!!");
        }
    }
}
