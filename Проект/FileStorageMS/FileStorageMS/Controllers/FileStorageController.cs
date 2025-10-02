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
        /// �������� ����������� ���� �� �������
        /// </summary>
        /// <remarks>
        /// ������ �������:
        /// 
        ///     GET
        ///     {
        ///         "name": "dark souls 3.jpg"
        ///     }
        ///
        /// </remarks>
        /// <param name="name">�������� ����� �����������</param>
        /// <returns>�����������</returns>
        /// <response code="200">�������� ����������</response>
        /// <response code="404">�� ������ ���� �����������</response>
        [HttpGet]
        public async Task<IResult> GetImage(string name)
        {
            logger.LogInformation($"{HttpContext.Request.Path}'s started at {DateTime.Now.ToLongTimeString()}");

            // ��������� �����������
            var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            var fileinfo = fileProvider.GetFileInfo("Images/" + name);

            // �������� �����������
            await HttpContext.Response.SendFileAsync(fileinfo);

            logger.LogInformation($"{HttpContext.Request.Path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Ok();
        }
    }
}
