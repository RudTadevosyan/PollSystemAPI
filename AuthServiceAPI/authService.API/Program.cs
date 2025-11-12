using System.Text;
using authService.API.CustomMiddlewares;
using authService.API.Extensions;
using authService.Application.Consumers;
using authService.Application.Helpers.ProfileMappers;
using authService.Application.Services;
using authService.Domain.Entities;
using authService.Domain.Interfaces;
using authService.Infrastructure.DataBases;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace authService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Db
        var conn = builder.Configuration.GetConnectionString("AuthServiceDbContext");
        builder.Services.AddDbContext<AuthServiceDbContext>
            (opt => opt.UseNpgsql(conn, x => x.MigrationsAssembly("authService.Infrastructure")));

        //Auth
        builder.Services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<AuthServiceDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

        //AutoMapper
        builder.Services.AddAutoMapper(_ => { }, typeof(UserProfile));

        //DI
        builder.Services.AddScoped<IAuthJwtService, AuthJwtService>();
        
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<GetUserEmailRequestConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConfig["Host"], h =>
                {
                    h.Username(rabbitMqConfig["Username"]!);
                    h.Password(rabbitMqConfig["Password"]!);
                });
                
                cfg.ReceiveEndpoint("get-user-emails-queue", e =>
                {
                    e.ConfigureConsumer<GetUserEmailRequestConsumer>(context);
                });
                
            });
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        //swagger 

        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations(); /////
        }

        app.UseMiddleware<GlobalExceptionHandler>(); /////

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}