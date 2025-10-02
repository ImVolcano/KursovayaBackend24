using DatabaseMS.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShopMS.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ ����������� �� ����� ������������
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// ������������� ���� ��� ������������
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddHostedService<RabbitMQConsumer>();

builder.Services.AddCors(); // ��������� ������� CORS

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Database API", Version = "v1" });

    c.IncludeXmlComments(AppContext.BaseDirectory + Assembly.GetExecutingAssembly().GetName().Name + ".xml");
});

var app = builder.Build();

// ���������� ��������� CORS ��� ���� ��������
app.UseCors(builder => builder.AllowAnyOrigin());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
