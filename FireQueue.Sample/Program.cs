using FireQueue.Core;
using System;

namespace FireQueue.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Initial Catalog=FireQueue;Integrated Security=SSPI;";

            var queueRepository = new MessageQueueRepository(connectionString);
            queueRepository.RegisterQueue("Test Queue");
            queueRepository.RegisterQueue("Test Queue 2");

            var queue = queueRepository.Get("Test Queue");
            queue.Subscribe<TestMessage>();
            queue.Publish(new TestMessage
            {
                TestString = "Hello World",
                TestInt = 123,
                DatePublished = DateTime.Now
            });

            Console.ReadLine();
        }
    }
}
