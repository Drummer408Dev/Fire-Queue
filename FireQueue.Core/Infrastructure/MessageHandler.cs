namespace FireQueue.Core.Infrastructure
{
    public abstract class MessageHandler<T> : IMessageHandler where T : IMessage
    {
        void IMessageHandler.Handle(IMessage message)
        {
            Handle((T)message);
        }

        public abstract void Handle(T message);
    }
}
