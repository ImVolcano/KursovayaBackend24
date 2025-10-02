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
        /// ��������� ���� ��� �� ���� ������
        /// </summary>
        /// <remarks>
        /// ������ �������
        ///     
        ///     GET
        ///     {
        ///         
        ///     }
        ///     
        /// </remarks>
        /// <param></param>
        /// <returns>������ ���� ��� �� ���� ������</returns>
        /// <response code="200">�������� ���������� ������ � ��������� �������</response>
        /// <response code="400">������ �� ����� �� �����-���� �������</response>
        [HttpGet]
        public async Task<IResult> GetGames()
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // �������� ���������
            producer.SendGameMessage();

            // ��������� ������
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            Game[] games = await consumer.CheckGameListener();

            // �����������, ���� ������ �� ����� �� �����-���� �������
            if(games == null)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.StatusCode(400);
            }

            // ������� ������� ���
            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Json(games);
        }

        /// <summary>
        /// ��������� ���� DLC �� ���� ������
        /// </summary>
        /// <remarks>
        /// ������ �������
        ///     
        ///     GET
        ///     {
        ///         
        ///     }
        ///     
        /// </remarks>
        /// <param></param>
        /// <returns>������ ���� DLC �� ���� ������</returns>
        /// <response code="200">�������� ���������� ������ � ��������� �������</response>
        /// <response code="400">������ �� ����� �� �����-���� �������</response>
        [HttpGet]
        public async Task<IResult> GetDLCs()
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // �������� ���������
            producer.SendDLCMessage();

            // ��������� ������
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

            // �������� ���������
            producer.SendBuyGameMessage(id);

            // ��������� ������
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            Game res = await consumer.CheckBuyGameListener();

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return res;
        }

        [HttpGet]
        public async Task<DLC> GetDLC(int id)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // �������� ���������
            producer.SendBuyDLCMessage(id);

            RabbitMQConsumer consumer = new RabbitMQConsumer();
            DLC res = await consumer.CheckBuyDLCListener();

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return res;
        }

        /// <summary>
        /// ���������� �������� ������� �������������
        /// </summary>
        /// <remarks>
        /// ������ �������
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
        /// <returns>������ ���</returns>
        /// <response code="200">�������� ���������� ������ � ���������� � ����������� ������ � ���� ������</response>
        /// <response code="400">������� �����</response>
        /// <response code="404">������ �� ID �� ������� �� ���������� ��� � ��� � �������</response>
        /// <response code="406">�� ������� ������������ ������������ �������</response>
        [HttpPost]
        public async Task<IResult> MakeTransaction(User user)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // ���������� ����� �������, ������� � ���������� � ����������
            int cart_sum = 0;
            List<int>? games = user.Cart_Games;
            List<int>? dlcs = user.Cart_DLCs;
            List<TransactionInfo> tInfos = new List<TransactionInfo>();

            // �����������, ���� ������� �����
            if (games.Count == 0 && dlcs.Count == 0)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "������� �����" });
            }

            // �����������, ���� � ������� ��� ���
            if (games.Count != 0)
            {
                for (int i = 0; i < games.Count; i++)
                {
                    Game game = await GetGame(games[i]);

                    if (game != null)
                    {
                        // ������������ ��� ���
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

            // �����������, ���� � ������� ��� DLC
            if (dlcs.Count != 0)
            {
                for (int i = 0; i < dlcs.Count; i++)
                {
                    DLC dlc = await GetDLC(dlcs[i]);

                    if (dlc != null)
                    {
                        // ������������ ��� DLC
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

            // �������� ������� ������������
            if (user.Balance < cart_sum)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {406}");
                return Results.StatusCode(406);
            }
            else
            {
                user.Balance -= cart_sum;
            }

            // �������� ��������� � ��� ��������
            Dictionary<int, List<TransactionInfo>> dict = new Dictionary<int, List<TransactionInfo>>();
            dict.Add(user.Id, tInfos);
            producer.SendBuyOperationMessage(dict);

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // ������� ������-����
            return Results.Ok();
        }
    }
}
