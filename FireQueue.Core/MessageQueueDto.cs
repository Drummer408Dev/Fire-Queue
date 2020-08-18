using System;

namespace FireQueue.Core
{
    internal class MessageQueueDto
    {
        public int QueueId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
