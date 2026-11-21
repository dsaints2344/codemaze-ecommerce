using Basket.Service.Endpoints;
using Basket.Service.Infrastructure.Data;
using Basket.Service.Infrastructure.RabbitMq;
using ECommerce.Shared.Infrastructure.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBasketStore, InMemoryBasketStore>();
builder.Services.AddRabbitMqEventBus(builder.Configuration);
builder.Services.AddHostedService<RabbitMqHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.RegisterEndpoints();

app.UseHttpsRedirection();

app.Run();
