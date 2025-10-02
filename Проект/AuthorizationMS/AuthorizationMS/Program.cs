using AuthorizationMS.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopMS.Logging;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ������������� ���� ��� ������������
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
builder.Services.AddCors(); // ��������� ������� CORS

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });
// Add services to the container.
builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authorization API", Version = "v1" });

    c.IncludeXmlComments(AppContext.BaseDirectory + Assembly.GetExecutingAssembly().GetName().Name + ".xml");
});

var app = builder.Build();

// ���������� ��������� CORS ��� ���� ��������
app.UseCors(builder => builder.AllowAnyOrigin());

app.UseAuthentication();

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

public class AuthOptions
{
    public const string ISSUER = "AuthMS"; // �������� ������
    public const string AUDIENCE = "AuthClient"; // ����������� ������
    const string KEY = "qwertyuiopasdfghjklzxcvbnm1234567890-=/.";   // ���� ��� ��������
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}