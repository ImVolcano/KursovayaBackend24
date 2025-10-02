using Microsoft.OpenApi.Models;
using ShopMS.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "Images" });

builder.Services.AddCors(); // добавляем сервисы CORS
// Add services to the container.
// устанавливаем файл для логгирования
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Storage API", Version = "v1" });

    c.IncludeXmlComments(AppContext.BaseDirectory + Assembly.GetExecutingAssembly().GetName().Name + ".xml");
});

var app = builder.Build();

// глобальная настройка CORS для всех ресурсов
app.UseCors(builder => builder.AllowAnyOrigin());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
