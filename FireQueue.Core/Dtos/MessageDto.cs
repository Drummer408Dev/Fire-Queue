using System;

namespace FireQueue.Core.Dtos
{
    internal class MessageDto
    {
        public int MessageId { get; set; }
        public string Contents { get; set; }
        public int QueueId { get; set; }
        public DateTime DatePublished { get; set; }
    }
}
