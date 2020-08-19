namespace FireQueue.Core.Infrastructure
{
    internal interface IMessageHandler
    {
        void Handle(IMessage message);
    }
}
