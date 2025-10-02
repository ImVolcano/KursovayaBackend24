namespace AuthorizationMS.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendGameMessage();
        public void SendDLCMessage();
        public void SendBuyGameMessage<T>(T message);
        public void SendBuyDLCMessage<T>(T message);
        public void SendBuyOperationMessage<T>(T message);
    }
}
