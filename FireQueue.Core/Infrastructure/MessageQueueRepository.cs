using FireQueue.Core.Database;
using FireQueue.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FireQueue.Core.Infrastructure
{
    public class MessageQueueRepository
    {
        private SqlClient sqlClient;
        private Dictionary<string, MessageQueue> messageQueues;
        private SqlInstaller sqlInstaller;

        public MessageQueueRepository(string connectionString)
        {
            sqlClient = new SqlClient(connectionString);
            messageQueues = new Dictionary<string, MessageQueue>();
            sqlInstaller = new SqlInstaller(sqlClient);

            sqlInstaller.Install();
            LoadExistingQueues();
        }

        private void LoadExistingQueues()
        {
            var sql = "SELECT * FROM Queues";
            var queues = sqlClient.Query<MessageQueueDto>(sql).ToList();

            foreach (var queue in queues)
                messageQueues.Add(queue.Name, new MessageQueue(queue.QueueId, sqlClient));
        }

        public void RegisterQueue(string queue)
        {
            if (!messageQueues.ContainsKey(queue))
            {
                var sql = "INSERT INTO Queues (Name, DateCreated) OUTPUT INSERTED.QueueId VALUES (@Name, GETDATE())";

                var queueId = sqlClient.QuerySingle<int>(sql, new
                {
                    Name = queue
                });

                messageQueues.Add(queue, new MessageQueue(queueId, sqlClient));
            }
        }

        public MessageQueue Get(string queue)
        {
            if (!messageQueues.ContainsKey(queue))
                throw new Exception($"Queue {queue} does not exist.");

            return messageQueues[queue];
        }
    }
}
