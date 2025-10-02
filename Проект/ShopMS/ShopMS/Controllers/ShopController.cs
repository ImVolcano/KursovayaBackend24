using AuthorizationMS.RabbitMQ;
using GameShopModel;
using Microsoft.AspNetCore.Mvc;
using ShopMS.Logging;
using System.Net.Http;

namespace ShopMS.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ShopController : ControllerBase
    {
        private IRabbitMQProducer producer;
        private ILogger<ShopController> logger;

        public ShopController(IRabbitMQProducer producer, ILogger<ShopController> logger)
        {
            this.producer = producer;
            this.logger = logger;
        }

        /// <summary>
        /// Получение всех игр из базы данных
        /// </summary>
        /// <remarks>
        /// Пример запроса
        ///     
        ///     GET
        ///     {
        ///         
        ///     }
        ///     
        /// </remarks>
        /// <param></param>
        /// <returns>Массив всех игр из базы данных</returns>
        /// <response code="200">Успешное выполнение метода с возвратом массива</response>
        /// <response code="400">Данные не дошли по какой-либо причине</response>
        [HttpGet]
        public async Task<IResult> GetGames()
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Отправка сообщения
            producer.SendGameMessage();

            // Получение ответа
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            Game[] games = await consumer.CheckGameListener();

            // Срабатывает, если данные не дошли по какой-либо причине
            if(games == null)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.StatusCode(400);
            }

            // Возврат массива игр
            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Json(games);
        }

        /// <summary>
        /// Получение всех DLC из базы данных
        /// </summary>
        /// <remarks>
        /// Пример запроса
        ///     
        ///     GET
        ///     {
        ///         
        ///     }
        ///     
        /// </remarks>
        /// <param></param>
        /// <returns>Массив всех DLC из базы данных</returns>
        /// <response code="200">Успешное выполнение метода с возвратом массива</response>
        /// <response code="400">Данные не дошли по какой-либо причине</response>
        [HttpGet]
        public async Task<IResult> GetDLCs()
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Отправка сообщения
            producer.SendDLCMessage();

            // Получение ответа
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            DLC[] dlcs = await consumer.CheckDLCListener();

            if (dlcs == null)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.StatusCode(400);
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Json(dlcs);
        }

        [HttpGet]
        public async Task<Game> GetGame(int id)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Отправка сообщения
            producer.SendBuyGameMessage(id);

            // Получение ответа
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            Game res = await consumer.CheckBuyGameListener();

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return res;
        }

        [HttpGet]
        public async Task<DLC> GetDLC(int id)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Отправка сообщения
            producer.SendBuyDLCMessage(id);

            RabbitMQConsumer consumer = new RabbitMQConsumer();
            DLC res = await consumer.CheckBuyDLCListener();

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return res;
        }

        /// <summary>
        /// Соверешние операции покупки пользователем
        /// </summary>
        /// <remarks>
        /// Пример запроса
        ///     
        ///     POST
        ///     {
        ///         "id": 2,
        ///         "Username": "ImVolkano",
        ///         "Email": "maks_kasimov@internet.ru,
        ///         "Password": "1234567890",
        ///         "Balance": 5000,
        ///         "Cart_Games": [ 1, 3 ],
        ///         "Cart_DLCs": [ 2 ]
        ///     }
        ///     
        /// </remarks>
        /// <param></param>
        /// <returns>Статус код</returns>
        /// <response code="200">Успешное выполнение метода с изменением и добавлением данных в базу данных</response>
        /// <response code="400">Корзина пуста</response>
        /// <response code="404">Товара по ID из корзины не существует или её нет в наличии</response>
        /// <response code="406">На балансе пользователя недостаточно средств</response>
        [HttpPost]
        public async Task<IResult> MakeTransaction(User user)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Объявление суммы покупки, корзины и ифнормации о транзакции
            int cart_sum = 0;
            List<int>? games = user.Cart_Games;
            List<int>? dlcs = user.Cart_DLCs;
            List<TransactionInfo> tInfos = new List<TransactionInfo>();

            // Срабатывает, если корзина пуста
            if (games.Count == 0 && dlcs.Count == 0)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Корзина пуста" });
            }

            // Срабатывает, если в корзине нет игр
            if (games.Count != 0)
            {
                for (int i = 0; i < games.Count; i++)
                {
                    Game game = await GetGame(games[i]);

                    if (game != null)
                    {
                        // Суммирование цен игр
                        cart_sum += game.Price;
                        tInfos.Add(new TransactionInfo() { Transaction_id = -1, Product_id = game.Id, Product_price = game.Price, Quantity = 1, Product_type = "Game"});
                    }
                    else
                    {
                        logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                        return Results.StatusCode(404);
                    }
                }
            }

            // Срабатывает, если в корзине нет DLC
            if (dlcs.Count != 0)
            {
                for (int i = 0; i < dlcs.Count; i++)
                {
                    DLC dlc = await GetDLC(dlcs[i]);

                    if (dlc != null)
                    {
                        // Суммирование цен DLC
                        cart_sum += dlc.Price;
                        tInfos.Add(new TransactionInfo() { Transaction_id = -1, Product_id = dlc.Id, Product_price = dlc.Price, Quantity = 1, Product_type = "DLC" });
                    }
                    else
                    {
                        logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                        return Results.StatusCode(404);
                    }
                }
            }

            // Проверка баланса пользователя
            if (user.Balance < cart_sum)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {406}");
                return Results.StatusCode(406);
            }
            else
            {
                user.Balance -= cart_sum;
            }

            // Создание сообщения и его отправка
            Dictionary<int, List<TransactionInfo>> dict = new Dictionary<int, List<TransactionInfo>>();
            dict.Add(user.Id, tInfos);
            producer.SendBuyOperationMessage(dict);

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Вовзрат статус-кода
            return Results.Ok();
        }
    }
}
