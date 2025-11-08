using authService.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Events;

namespace authService.Application.Consumers;

public class GetUserEmailRequestConsumer : IConsumer<GetUserEmailRequest>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GetUserEmailRequestConsumer> _logger;

    public GetUserEmailRequestConsumer(UserManager<User> userManager, ILogger<GetUserEmailRequestConsumer> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetUserEmailRequest> context)
    {
        var userIds = context.Message.UserIds;
        var result = new Dictionary<int, string>();

        _logger.LogInformation($"Fetching emails for {userIds.Count} userIds");

        List<User?> users = new();
        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null) users.Add(user);
        }
        
        foreach (var user in users)
        {
            if (!string.IsNullOrEmpty(user?.Email))
            {
                result[user.Id] = user.Email;
            }
        }

        // Send
        await context.RespondAsync(new GetUserEmailResponse(result));

        _logger.LogInformation($"Responded with {result.Count} emails");
    }
}