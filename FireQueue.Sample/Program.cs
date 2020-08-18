using FireQueue.Core;
using System;

namespace FireQueue.Sample
{
    class TestMessage : IMessage
    {
        public string TestString { get; set; }
        public int TestInt { get; set; }
        public DateTime DatePublished { get; set; }
    }

    class TestMessageHandler : MessageHandler<TestMessage>
    {
        public override void Handle(TestMessage message)
        {
            Console.WriteLine("I HAVE BEEN HANDLED!!");
        }
    }

    class AnotherTestMessageHandler : MessageHandler<TestMessage>
    {
        public override void Handle(TestMessage message)
        {
            Console.WriteLine("ANOTHER HANDLE DAWGGGGG!!!!!!!!!!!!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Initial Catalog=FireQueue;Integrated Security=SSPI;";

            var queueRepository = new MessageQueueRepository(connectionString);
            //queueRepository.RegisterQueue("Test Queue");
            //queueRepository.RegisterQueue("Test Queue 2");
            queueRepository.Subscribe<TestMessage>("Test Queue");
            queueRepository.Publish("Test Queue 2", new TestMessage
            {
                TestString = "Hello World",
                TestInt = 123,
                DatePublished = DateTime.Now
            });

            Console.WriteLine("\n\nDONE!");
            Console.ReadLine();
        }
    }
}
