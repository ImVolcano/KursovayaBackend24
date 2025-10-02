using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace AuthorizationMS.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public void SendUserMessage<T>(T message)
        {
            // Получение соединения с сервером брокера
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            // Создание канала
            using
            var channel = connection.CreateModel();

            // Настройка сообщения для получения ответа на него
            string correlationId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = "userReply";

            // Объявление очереди
            channel.QueueDeclare("user", exclusive: false, autoDelete: false);

            // Подготовка и отправка сообщения в очередь
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "user", body: body, basicProperties: props);
        }
    }
}
