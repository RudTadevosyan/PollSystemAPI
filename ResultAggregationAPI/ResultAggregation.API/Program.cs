using MassTransit;
using MongoDB.Driver;
using ResultAggregation.Application.Consumers;
using ResultAggregation.Application.Services;
using ResultAggregation.Domain.Interfaces;
using ResultAggregation.Infrastructure.Cache;
using ResultAggregation.Infrastructure.Repositories;
using Shared.Events;
using StackExchange.Redis;

namespace ResultAggregation.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        
        // MongoDb
        var mongoConnectionString = builder.Configuration["MongoDb:ConnectionString"];
        var mongoDatabaseName = builder.Configuration["MongoDb:DatabaseName"];

        builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
        builder.Services.AddSingleton<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
            
        // HTTP
        builder.Services.AddHttpClient("PollManagement", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["PollManagement:BaseUrl"]!);
        });
        
        
        // DI
        builder.Services.AddScoped<IPollResultService, PollResultService>();
        builder.Services.AddScoped<IPollResultRepository, PollResultRepository>();
        
        // Redis + RMQ
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = builder.Configuration["Redis:ConnectionString"];
            return ConnectionMultiplexer.Connect(config!);
        });

        builder.Services.AddSingleton<RedisCacheService>();
        
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<PollClosedEventConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConfig["Host"], h =>
                {
                    h.Username(rabbitMqConfig["Username"]!);
                    h.Password(rabbitMqConfig["Password"]!);
                });
                
                cfg.ReceiveEndpoint("poll-closed-queue", e =>
                {
                    e.ConfigureConsumer<PollClosedEventConsumer>(context);
                });
                
            });
            
            x.AddRequestClient<GetVotersRequest>();
            x.AddRequestClient<GetUserEmailRequest>();
        });
        
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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
    }
}