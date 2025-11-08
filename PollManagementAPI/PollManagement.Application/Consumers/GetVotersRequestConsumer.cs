using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PollManagement.Infrastructure.DataBases;
using Shared.Events;

namespace PollManagement.Application.Consumers;

public class GetVotersRequestConsumer : IConsumer<GetVotersRequest>
{
    private readonly ILogger<GetVotersRequestConsumer> _logger;
    private readonly VotePollDbContext _context;
    
    public GetVotersRequestConsumer(ILogger<GetVotersRequestConsumer> logger, VotePollDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<GetVotersRequest> context)
    {
        var pollId = context.Message.PollId;
        
        _logger.LogInformation("Received GetVotersRequest");
        var userIds = await _context.Votes
            .AsNoTracking()
            .Where(v => v.PollId == pollId)
            .Select(v => v.UserId)
            .Distinct()
            .ToListAsync();
        
        await context.RespondAsync(new GetVotersResponse(pollId, userIds));
        
        _logger.LogInformation($"Responded with {userIds.Count} userIds");
    }
}