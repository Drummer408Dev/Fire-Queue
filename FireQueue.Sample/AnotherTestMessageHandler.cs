using FireQueue.Core;
using System;

namespace FireQueue.Sample
{
    class AnotherTestMessageHandler : MessageHandler<TestMessage>
    {
        public override void Handle(TestMessage message)
        {
            Console.WriteLine("ANOTHER HANDLE DAWGGGGG!!!!!!!!!!!!");
        }
    }
}
