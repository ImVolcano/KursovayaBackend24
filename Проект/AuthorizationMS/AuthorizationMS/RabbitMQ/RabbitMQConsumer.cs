using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace AuthorizationMS.RabbitMQ
{
    public class RabbitMQConsumer
    {
        public int? CheckListener()
        {
            int? res = null;

            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();
            channel.QueueDeclare("userReply", exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                res = JsonSerializer.Deserialize<int>(message);
            };

            channel.BasicConsume(queue: "userReply", autoAck: true, consumer: consumer);

            while (res == null)
            {

            }

            return res;
        }
    }
}
