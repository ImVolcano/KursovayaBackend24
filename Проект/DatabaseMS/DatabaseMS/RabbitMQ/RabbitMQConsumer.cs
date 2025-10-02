using DatabaseMS.Controllers;
using GameShopModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopMS.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DatabaseMS.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer()
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqps://vzxubjvz:yuAgs2zwI9NkudJbPsV7RPj0peD5LQmv@cow.rmq2.cloudamqp.com/vzxubjvz") };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "user", exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "game", exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "dlc", exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "buyGame", exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "buyDLC", exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "buyOper", exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            // Создание экземпляра контроллера для работы с базой данных
            DbContextOptions<ApplicationContext> options = new DbContextOptions<ApplicationContext>();
            ApplicationContext db = new ApplicationContext(options);
            ILogger<DatabaseController> logger = new Logger<DatabaseController>(new LoggerFactory());
            DatabaseController databaseController = new DatabaseController(db, logger);

            // Обработчик входящих сообщений для таблицы Users
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                int res = 400;

                // Получение свойств входящего сообщения
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                // Получение данных из сообщения
                var content = Encoding.UTF8.GetString(body);
                Dictionary<string, User> dict = JsonSerializer.Deserialize<Dictionary<string, User>>(content);

                // Обработка данных
                if (dict.ContainsKey("create"))
                {
                    User user = dict["create"];

                    var status = (StatusCodeHttpResult)await databaseController.CreateUser(user);
                    res = status.StatusCode;
                } 
                else if (dict.ContainsKey("check"))
                {
                    User user = dict["check"];
                    var status = (StatusCodeHttpResult)await databaseController.CheckUser(user.Username, user.Password);
                    res = status.StatusCode;
                }

                // Формирование и отправка ответа на сообщение
                var json = JsonSerializer.Serialize(res);
                var responseBytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("user", false, consumer);

            // Обработчик входящих сообщений для таблицы Games
            var consumerGames = new EventingBasicConsumer(_channel);
            consumerGames.Received += async (ch, ea) =>
            {
                // Получение свойств входящего сообщения
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                // Обработка данных
                var res = await databaseController.GetAllGames();

                // Формирование и отправка ответа на сообщение
                var json = JsonSerializer.Serialize(res);
                var responseBytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("game", false, consumerGames);

            // Обработчик входящих сообщений для таблицы DLCs
            var consumerDLCs = new EventingBasicConsumer(_channel);
            consumerDLCs.Received += async (ch, ea) =>
            {
                // Получение свойств входящего сообщения
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                // Обработка данных
                var res = await databaseController.GetAllDLCs();

                // Формирование и отправка ответа на сообщение
                var json = JsonSerializer.Serialize(res);
                var responseBytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("dlc", false, consumerDLCs);

            var consumerBuyGame = new EventingBasicConsumer(_channel);
            consumerBuyGame.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                
                var content = Encoding.UTF8.GetString(body);
                int id = JsonSerializer.Deserialize<int>(content);

                Game game = await databaseController.GetGame(id);
                var json = JsonSerializer.Serialize(game);
                var responseBytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("buyGame", false, consumerBuyGame);

            var consumerBuyDLC = new EventingBasicConsumer(_channel);
            consumerBuyDLC.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var content = Encoding.UTF8.GetString(body);
                int id = JsonSerializer.Deserialize<int>(content);

                DLC dlc = await databaseController.GetDLC(id);
                var json = JsonSerializer.Serialize(dlc);
                var responseBytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("buyDLC", false, consumerBuyDLC);

            // Обработчик сообщений для совершения операции покупки
            var consumerBuyOper = new EventingBasicConsumer(_channel);
            consumerBuyOper.Received += async (ch, ea) =>
            {
                // Получение свойств входящего сообщения
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                // Получение данных входящего сообщения
                var content = Encoding.UTF8.GetString(body);
                Dictionary<int, List<TransactionInfo>> dict = JsonSerializer.Deserialize<Dictionary<int, List<TransactionInfo>>>(content);

                // Обработка данных для совершения операции покупки
                int user_id = -1;
                List<TransactionInfo> tInfos = null;

                foreach(var item in dict)
                {
                    user_id = item.Key;
                    tInfos = item.Value;
                }

                Transaction transaction = new Transaction() { User_id = user_id };
                await databaseController.CreateTransaction(transaction);

                User user = await databaseController.GetUser(user_id);
                user.Cart_DLCs.Clear();
                user.Cart_Games.Clear();

                int tId = await databaseController.GetLastTransaction();

                foreach(var item in tInfos)
                {
                    item.Transaction_id = tId;
                    await databaseController.CreateTransactionInfo(item);

                    user.Balance -= item.Product_price;

                    if(item.Product_type == "Game")
                    {
                        Game game = await databaseController.GetGame(item.Product_id);
                        game.Quantity--;
                        await databaseController.UpdateGame(game);
                    }
                    else
                    {
                        DLC dlc = await databaseController.GetDLC(item.Product_id);
                        dlc.Quantity--;
                        await databaseController.UpdateDLC(dlc);
                    }
                }

                await databaseController.UpdateUser(user);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("buyOper", false, consumerBuyOper);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
