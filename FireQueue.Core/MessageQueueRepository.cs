using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FireQueue.Core
{
    public class MessageQueueRepository
    {
        private SqlClient sqlClient;
        private Dictionary<string, int> queueIds;
        private Dictionary<string, Dictionary<Type, List<Type>>> queueSubscribers;

        public MessageQueueRepository(string connectionString)
        {
            sqlClient = new SqlClient(connectionString);
            queueIds = new Dictionary<string, int>();
            queueSubscribers = new Dictionary<string, Dictionary<Type, List<Type>>>();

            LoadQueueIds();
        }

        private void LoadQueueIds()
        {
            var sql = "SELECT * FROM Queues";
            var queues = sqlClient.Query<MessageQueue>(sql).ToList();
            queues.ForEach(q => queueIds.Add(q.Name, q.QueueId));
        }

        public void RegisterQueue(string queue)
        {
            if (!queueIds.ContainsKey(queue))
                throw new Exception($"Queue {queue} has already been registered.");

            var sql = "INSERT INTO Queues (Name, DateCreated) OUTPUT INSERTED.QueueId VALUES (@Name, GETDATE())";

            var queueId = sqlClient.QuerySingle<int>(sql, new
            {
                Name = queue
            });

            queueIds.Add(queue, queueId);
        }

        public void Publish(string queue, IMessage message)
        {
            var sql = "INSERT INTO Messages (Contents, QueueId, DatePublished) VALUES (@Contents, @QueueId, GETDATE())";
            var queueId = queueIds[queue];

            var messageType = message.GetType().Name;
            var messageContents = JsonConvert.SerializeObject(message);

            var serializationMessage = new SerializationMessage
            {
                Type = messageType,
                Contents = messageContents
            };

            sqlClient.ExecuteSql(sql, new
            {
                Contents = JsonConvert.SerializeObject(serializationMessage),
                QueueId = queueId
            });
        }

        public void Subscribe<T>(string queue) where T : IMessage
        {
            if (!queueSubscribers.ContainsKey(queue))
                queueSubscribers.Add(queue, new Dictionary<Type, List<Type>>());

            var messageType = typeof(T);
            var handlerTypes = Assembly.GetCallingAssembly().GetTypes().Where(t => t.BaseType == typeof(MessageHandler<T>)).ToList();
            if (!queueSubscribers[queue].ContainsKey(messageType))
                queueSubscribers[queue].Add(messageType, handlerTypes);

            Task.Factory.StartNew(() => ProcessSubscription<T>(queue));
        }

        private void ProcessSubscription<T>(string queue) where T : IMessage
        {
            var selectSql = "SELECT * FROM Messages WHERE QueueId = @QueueId AND DatePublished = (SELECT MAX(DatePublished) FROM Messages WHERE QueueId = @QueueId)";
            var deleteSql = "DELETE FROM Messages WHERE MessageId = @MessageId";
            var queueId = queueIds[queue];

            while (true)
            {
                var serializedMessage = sqlClient.Query<Message>(selectSql, new
                {
                    QueueId = queueId
                }).SingleOrDefault();

                if (serializedMessage != null)
                {
                    var serializationMessage = JsonConvert.DeserializeObject<SerializationMessage>(serializedMessage.Contents);
                    var message = JsonConvert.DeserializeObject<T>(serializationMessage.Contents);

                    var handlerTypes = queueSubscribers[queue][typeof(T)];
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = (MessageHandler<T>) Activator.CreateInstance(handlerType);
                        handler.Handle(message);
                    }

                    sqlClient.ExecuteSql(deleteSql, new
                    {
                        MessageId = serializedMessage.MessageId
                    });
                }
            }
        }
    }
}
