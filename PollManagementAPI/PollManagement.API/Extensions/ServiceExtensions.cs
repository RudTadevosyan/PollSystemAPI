using PollManagement.Application.Helpers.Mappings;
using PollManagement.Application.Messaging.Publishers;
using PollManagement.Application.Services;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using PollManagement.Infrastructure.Repositories;

namespace PollManagement.API.Extensions;

public static class ServiceExtensions
{
    public static void AddDependencyInjection(this IServiceCollection services)
    {
        // repositories
        services.AddScoped<IPollRepository, PollRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IOptionRepository, OptionsRepository>();
        
        // services
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IOptionService, OptionService>();
        
        // automapper
        services.AddAutoMapper(_ => { }, typeof(PollProfile));
        services.AddAutoMapper(_ => { }, typeof(OptionProfile));
        services.AddAutoMapper(_ => { }, typeof(VoteProfile));
        
        // events
        services.AddScoped<PollClosedPublisher>();
        
        services.AddAuthorization();


    }
}