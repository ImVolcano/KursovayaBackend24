using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace AuthorizationMS.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public void SendGameMessage()
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();

            string correlationId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = "gameReply";

            channel.QueueDeclare("game", exclusive: false, autoDelete: false);
            var json = JsonSerializer.Serialize("Games");
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "game", body: body, basicProperties: props);
        }

        public void SendDLCMessage()
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();

            string correlationId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = "dlcReply";

            channel.QueueDeclare("dlc", exclusive: false, autoDelete: false);
            var json = JsonSerializer.Serialize("DLCs");
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "dlc", body: body, basicProperties: props);
        }

        public void SendBuyGameMessage<T>(T message)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();

            string correlationId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = "buyGameReply";

            channel.QueueDeclare("buyGame", exclusive: false, autoDelete: false);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "buyGame", body: body, basicProperties: props);
        }

        public void SendBuyDLCMessage<T>(T message)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();

            string correlationId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = "buyDLCReply";

            channel.QueueDeclare("buyDLC", exclusive: false, autoDelete: false);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "buyDLC", body: body, basicProperties: props);
        }

        public void SendBuyOperationMessage<T>(T message)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            var connection = factory.CreateConnection();

            using
            var channel = connection.CreateModel();

            string correlationId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = "buyOperReply";

            channel.QueueDeclare("buyOper", exclusive: false, autoDelete: false);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "buyOper", body: body, basicProperties: props);
        }
    }
}
