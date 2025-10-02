using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using GameShopModel;

namespace AuthorizationMS.RabbitMQ
{
    public class RabbitMQConsumer
    {
        public async Task<Game[]> CheckGameListener()
        {
            Game[] res = null;

            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();
            channel.QueueDeclare("gameReply", exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                res = JsonSerializer.Deserialize<Game[]>(message);
            };

            channel.BasicConsume(queue: "gameReply", autoAck: true, consumer: consumer);

            while (res == null)
            {

            }

            return res;
        }

        public async Task<DLC[]> CheckDLCListener()
        {
            DLC[] res = null;

            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();
            channel.QueueDeclare("dlcReply", exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                res = JsonSerializer.Deserialize<DLC[]>(message);
            };

            channel.BasicConsume(queue: "dlcReply", autoAck: true, consumer: consumer);

            while (res == null)
            {

            }

            return res;
        }

        public async Task<Game> CheckBuyGameListener()
        {
            Game res = null;

            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();
            channel.QueueDeclare("buyGameReply", exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                res = JsonSerializer.Deserialize<Game>(message);
            };

            channel.BasicConsume(queue: "buyGameReply", autoAck: true, consumer: consumer);

            while (res == null)
            {

            }

            return res;
        }

        public async Task<DLC> CheckBuyDLCListener()
        {
            DLC res = null;

            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();
            channel.QueueDeclare("buyDLCReply", exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                res = JsonSerializer.Deserialize<DLC>(message);
            };

            channel.BasicConsume(queue: "buyDLCReply", autoAck: true, consumer: consumer);

            while (res == null)
            {

            }

            return res;
        }
    }
}
