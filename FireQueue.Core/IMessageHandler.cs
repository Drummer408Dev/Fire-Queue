namespace FireQueue.Core
{
    internal interface IMessageHandler
    {
        void Handle(IMessage message);
    }
}
