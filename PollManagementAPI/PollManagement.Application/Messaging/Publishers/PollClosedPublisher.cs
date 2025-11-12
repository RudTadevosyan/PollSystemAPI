using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Events;

namespace PollManagement.Application.Messaging.Publishers;

public class PollClosedPublisher
{
    private readonly ILogger<PollClosedPublisher> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public PollClosedPublisher(ILogger<PollClosedPublisher> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync(PollClosedEvent @event)
    {
        try
        {
            _logger.LogInformation($"Publishing PollClosedEvent for PollId: {@event.PollResult.PollId}");
            await _publishEndpoint.Publish(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing PollClosedEvent for PollId {@event.PollResult.PollId}");
            throw;
        }
    }
}