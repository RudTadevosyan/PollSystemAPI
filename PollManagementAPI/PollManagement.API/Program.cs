using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PollManagement.API.Extensions;
using PollManagement.API.Middlewares;
using PollManagement.Application.Background.Jobs;
using PollManagement.Application.Consumers;
using PollManagement.Infrastructure.DataBases;
using Quartz;

namespace PollManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DataBase connection
        var cn = builder.Configuration.GetConnectionString("VotePollConnection");
        builder.Services.AddDbContext<VotePollDbContext>
            (options => options.UseNpgsql(cn, x => x.MigrationsAssembly("PollManagement.Infrastructure")));
        
        
        //Auth
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });
        
        builder.Services.AddControllers();
        
        // Dependency injection 
        builder.Services.AddDependencyInjection();

        
        // RMQ (MassTransit) + Quartz 
        
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<GetVotersRequestConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                
                cfg.Host(rabbitMqConfig["Host"], h =>
                {
                    h.Username(rabbitMqConfig["Username"]!);
                    h.Password(rabbitMqConfig["Password"]!);
                });
                
                cfg.ReceiveEndpoint("get-poll-voters", e =>
                {
                    e.ConfigureConsumer<GetVotersRequestConsumer>(context);
                });
                
                
            });
        });
        
        builder.Services.AddQuartz(q =>
        {
            q.AddJob<ClosePollsJob>(j => j.WithIdentity("ClosePollsJob"));

            q.AddTrigger(t => t
                .ForJob("ClosePollsJob")
                .WithIdentity("ClosePollsTrigger")
                .WithSimpleSchedule(s => 
                    s.WithIntervalInMinutes(1)
                        .RepeatForever()));
        });

        builder.Services.AddQuartzHostedService(opt =>
        {
            // if we close the app and there is a job running
            // it will not kill the job until it is done, and after it will self dispose 
            opt.WaitForJobsToComplete = true;
        });
        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        
        // swagger
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PollManagement API",
                Version = "v1",
                Description = "API for managing polls and votes"
            });

            // JWT Bearer auth configuration
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    securityScheme, []
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations(); /////
        }
        
        app.UseMiddleware<ExceptionHandlerMiddleware>(); /////
        app.UseMiddleware<PollExpirationMiddleware>();

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}