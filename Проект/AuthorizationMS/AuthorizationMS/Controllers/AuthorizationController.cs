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
        /// ����������� ������������
        /// </summary>
        /// <remarks>
        /// ������ �������:
        /// 
        ///     POST
        ///     {
        ///        "username": "FareWell",
        ///        "email": "farewl@gmail.com",
        ///        "password": "qwerty"
        ///     }
        ///     
        /// </remarks>
        /// <param name="username">��� ������������</param>
        /// <param name="email">����������� �����</param>
        /// <param name="password">������</param>
        /// <returns>������-���</returns>
        /// <response code="200">�������� ���������� �����������</response>
        /// <response code="400">������������ ������</response>
        [HttpPost]
        public async Task<IResult> RegisterUser(string username, string email, string password)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // �������� ������������
            User user = new User() { Username = username, Email = email, Password = password };

            // �������� ��������� ��� �������
            Dictionary<string, User> message = new Dictionary<string, User>();
            message.Add("create", user);

            // �������� ���������
            producer.SendUserMessage(message);

            // ��������� ������
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            int? res = consumer.CheckListener();

            // �����������, ���� ����������� �� �������
            if(res == null || res == 400)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new {message = "������ �� ����� �����������"});
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // �����������, ���� ����������� ����������� �������
            return Results.StatusCode(200);
        }

        /// <summary>
        /// ����������� ������������
        /// </summary>
        /// <remarks>
        /// ������ �������:
        /// 
        ///     GET
        ///     {
        ///         "username": "ImVolkano",
        ///         "password": "1234567890"
        ///     }
        ///     
        /// </remarks>
        /// <param name="username">��� ������������</param>
        /// <param name="password">������</param>
        /// <returns>JWT-�����</returns>
        /// <response code="200">�������� �����������. ���������� JWT-�����.</response>
        /// <response code="400">������������ ������</response>
        /// <response code="404">������������ � ������ ������� �� ������</response>
        [HttpGet]
        public async Task<IResult> LoginUser(string username, string password)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // �������� ����������� ������������
            var res = (StatusCodeHttpResult) await IsUser(username, password);

            // �����������, ���� ������������ ���������������
            if(res.StatusCode == 200)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                // ������� JWT-�����
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // ��������� �����
                var response = new
                {
                    access_token = encodedJwt,
                    username = username
                };

                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
                return Results.Json(response);
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
            return Results.NotFound(new {message = "������ ������������ ���� � ���� ������"});
        }

        /// <summary>
        /// �������� ������� ������������ � ���� ������
        /// </summary>
        /// <remarks>
        /// ������ �������:
        /// 
        ///     GET
        ///     {
        ///         "username": "ImVolkano",
        ///         "password": "1234567890"
        ///     }
        /// </remarks>
        /// <param name="username">��� ������������</param>
        /// <param name="password">������</param>
        /// <returns>������-���</returns>
        /// <response code="200">������������ ������</response>
        /// <response code="400">������������ ������</response>
        /// <response code="404">������������ � ������ ������� �� ������</response>
        [HttpGet]
        public async Task<IResult> IsUser(string username, string password)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // �������� ��������� ��� �������� ������� ������������ � ���� ������
            User user = new User() { Username = username, Email = "", Password = password };
            Dictionary<string, User> message = new Dictionary<string, User>();
            message.Add("check", user);
            producer.SendUserMessage(message);

            // ��������� ������
            RabbitMQConsumer consumer = new RabbitMQConsumer();
            int? res = consumer.CheckListener();

            // �����������, ���� ������������ ���������������
            if (res == 200)
            {
                logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
                return Results.StatusCode(200);
            }

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
            // �����������, ���� ������������ �� ���������������
            return Results.StatusCode(404);
        }
    }
}
