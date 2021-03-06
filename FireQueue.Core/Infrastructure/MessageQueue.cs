﻿using FireQueue.Core.Database;
using FireQueue.Core.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FireQueue.Core.Infrastructure
{
    public class MessageQueue
    {
        private int queueId;
        private SqlClient sqlClient;
        private Dictionary<Type, List<Type>> subscribers;

        internal MessageQueue(int queueId, SqlClient sqlClient)
        {
            this.queueId = queueId;
            this.sqlClient = sqlClient;
            subscribers = new Dictionary<Type, List<Type>>();
        }

        public void Publish(IMessage message)
        {
            var serializationMessage = SerializeMessage(message);
            InsertMessageIntoDatabase(serializationMessage);
        }

        private SerializationMessage SerializeMessage(IMessage message)
        {
            var messageContents = JsonConvert.SerializeObject(message);
            return new SerializationMessage
            {
                Contents = messageContents
            };
        }

        private void InsertMessageIntoDatabase(SerializationMessage serializationMessage)
        {
            var sql = "INSERT INTO Messages (Contents, QueueId, DatePublished) VALUES (@Contents, @QueueId, GETDATE())";
            sqlClient.ExecuteSql(sql, new
            {
                Contents = JsonConvert.SerializeObject(serializationMessage),
                QueueId = queueId
            });
        }

        public void Subscribe<T>() where T : IMessage
        {
            var messageType = typeof(T);
            if (!subscribers.ContainsKey(messageType))
                subscribers.Add(messageType, new List<Type>());

            var handlerTypes = Assembly.GetEntryAssembly().GetTypes().Where(t => t.BaseType == typeof(MessageHandler<T>)).ToList();
            foreach (var handlerType in handlerTypes)
                subscribers[messageType].Add(handlerType);

            Task.Factory.StartNew(() => ProcessSubscription<T>());
        }

        // TODO: Refactor this out into its own class
        private void ProcessSubscription<T>() where T : IMessage
        {
            var selectSql = "SELECT * FROM Messages WHERE QueueId = @QueueId AND DatePublished = (SELECT MAX(DatePublished) FROM Messages WHERE QueueId = @QueueId)";
            var deleteSql = "DELETE FROM Messages WHERE MessageId = @MessageId";

            // TODO: Probably not the most efficient to keep hammering the database to check for new messages-
            //       need to look into more optimal ways of determining whether a new message was published
            while (true)
            {
                var serializedMessage = sqlClient.Query<MessageDto>(selectSql, new
                {
                    QueueId = queueId
                }).SingleOrDefault();

                if (serializedMessage != null)
                {
                    var serializationMessage = JsonConvert.DeserializeObject<SerializationMessage>(serializedMessage.Contents);
                    var message = JsonConvert.DeserializeObject<T>(serializationMessage.Contents);

                    var handlerTypes = subscribers[typeof(T)];
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = (MessageHandler<T>)Activator.CreateInstance(handlerType);
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
