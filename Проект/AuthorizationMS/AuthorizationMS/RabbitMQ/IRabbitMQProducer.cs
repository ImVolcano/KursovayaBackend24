namespace AuthorizationMS.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendUserMessage<T>(T message);
    }
}
