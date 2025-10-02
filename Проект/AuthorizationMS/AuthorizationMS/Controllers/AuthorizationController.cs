using AuthorizationMS.RabbitMQ;
using GameShopModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthorizationMS.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthorizationController : ControllerBase
    {
        private IRabbitMQProducer producer;
        private ILogger<AuthorizationController> logger;

        public AuthorizationController(IRabbitMQProducer rabbitMQProducer, ILogger<AuthorizationController> logger)
        {
            this.producer = rabbitMQProducer;
            this.logger = logger;
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST
        ///     {
        ///        "username": "FareWell",
        ///        "email": "farewl@gmail.com",
        ///        "password": "qwerty"
        ///     }
        ///     
        /// </remarks>
        /// <param name="username">Имя пользователя</param>
        /// <param name="email">Электронная почта</param>
        /// <param name="password">Пароль</param>
        /// <returns>Статус-код</returns>
        /// <response code="200">Успешное завершение регистрации</response>
        /// <response code="400">Некорректные данные</response>
        [HttpPost]
        public async Task<IResult> RegisterUser(string username, string email, string password)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Создание пользователя
            User user = new User() { Username = username, Email = email, Password = password };

            // Создание сообщения для брокера
            Dictionary<string, User> message = new Dictionary<string, User>();
            message.Add("create", user);

            // Отправка сообщения
            producer.SendUserMessage(message);

            // Получение ответа
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            int? res = consumer.CheckListener();

            // Срабатывает, если регистрация не удалась
            if(res == null || res == 400)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new {message = "Ошибка во время регистрации"});
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если регистрация завершилась успешно
            return Results.StatusCode(200);
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET
        ///     {
        ///         "username": "ImVolkano",
        ///         "password": "1234567890"
        ///     }
        ///     
        /// </remarks>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>JWT-токен</returns>
        /// <response code="200">Успешная авторизация. Возвращает JWT-токен.</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Пользователь с такими данными не найден</response>
        [HttpGet]
        public async Task<IResult> LoginUser(string username, string password)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Проверка регистрации пользователя
            var res = (StatusCodeHttpResult) await IsUser(username, password);

            // Срабатывает, если пользователь зарегистрирован
            if(res.StatusCode == 200)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // формируем ответ
                var response = new
                {
                    access_token = encodedJwt,
                    username = username
                };

                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
                return Results.Json(response);
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
            return Results.NotFound(new {message = "Такого пользователя нету в базе данных"});
        }

        /// <summary>
        /// Проверка наличия пользователя в базе данных
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET
        ///     {
        ///         "username": "ImVolkano",
        ///         "password": "1234567890"
        ///     }
        /// </remarks>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Статус-код</returns>
        /// <response code="200">Пользователь найден</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Пользователь с такими данными не найден</response>
        [HttpGet]
        public async Task<IResult> IsUser(string username, string password)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Отправка сообщения для проверки наличия пользователя в базе данных
            User user = new User() { Username = username, Email = "", Password = password };
            Dictionary<string, User> message = new Dictionary<string, User>();
            message.Add("check", user);
            producer.SendUserMessage(message);

            // Получение ответа
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            int? res = consumer.CheckListener();

            // Срабатывает, если пользователь зарегистрирован
            if (res == 200)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
                return Results.StatusCode(200);
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
            // Срабатывает, если пользователь не зарегистрирован
            return Results.StatusCode(404);
        }
    }
}
