using System;

namespace FireQueue.Core
{
    internal class MessageQueue
    {
        public int QueueId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }

    internal class Message
    {
        public int MessageId { get; set; }
        public string Contents { get; set; }
        public int QueueId { get; set; }
        public DateTime DatePublished { get; set; }
    }
}
