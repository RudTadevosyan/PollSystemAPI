using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PollManagement.Application.Messaging.Publishers;
using PollManagement.Infrastructure.DataBases;
using Quartz;
using Shared.DTOs.Option;
using Shared.DTOs.Poll;
using Shared.Events;

namespace PollManagement.Application.Background.Jobs;

public class ClosePollsJob : IJob
{
    private readonly VotePollDbContext _context;
    private readonly PollClosedPublisher _publisher;
    private readonly ILogger<ClosePollsJob> _logger;


    public ClosePollsJob(
        VotePollDbContext context, 
        PollClosedPublisher publisher, 
        ILogger<ClosePollsJob> logger
        )
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation($"Stating closing polls at {DateTime.UtcNow}");
            
            DateTime now = DateTime.UtcNow;
            var pollsToClose = await _context.Polls
                .Include(p => p.Options)
                .Where(p => p.Status == true && p.ClosesAt <= now).ToListAsync();
            
            // first publish then change , ideally better use special patterns for event atomicity
            foreach (var poll in pollsToClose)
            {
                var data = new PollResultDto()
                {
                    PollId = poll.PollId,
                    UserId = poll.UserId,
                    Title = poll.Title,
                    Description = poll.Description,
                    Category = poll.Category,
                    Topic = poll.Topic,
                    ClosedAt = now,
                    Options = poll.Options.Select(o => new OptionResultDto()
                    {
                        OptionId = o.OptionId,
                        Text = o.Text,
                        VoteCount = o.VoteCount
                    }).OrderBy(o => o.OptionId).ToList(),
                };

                await _publisher.PublishAsync(new PollClosedEvent()
                {
                    PollResult = data,
                });
            }
            
            foreach (var poll in pollsToClose)
            {
                poll.Status = false;
            }
            
            if(pollsToClose.Count > 0) await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Closed {pollsToClose.Count} polls");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in closing polls");
            throw;
        }
    }
}