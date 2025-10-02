using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace FileStorageMS.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileStorageController : ControllerBase
    {
        private ILogger<FileStorageController> logger;

        public FileStorageController(ILogger<FileStorageController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Отправка изображения игры по запросу
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET
        ///     {
        ///         "name": "dark souls 3.jpg"
        ///     }
        ///
        /// </remarks>
        /// <param name="name">Название файла изображения</param>
        /// <returns>Изображение</returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="404">Не найден файл изображения</response>
        [HttpGet]
        public async Task<IResult> GetImage(string name)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение изображения
            var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            var fileinfo = fileProvider.GetFileInfo("Images/" + name);

            // Отправка изображения
            await HttpContext.Response.SendFileAsync(fileinfo);

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Ok();
        }
    }
}
