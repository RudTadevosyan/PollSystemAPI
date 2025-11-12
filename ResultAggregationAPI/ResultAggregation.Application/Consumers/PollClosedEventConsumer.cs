using MassTransit;
using Microsoft.Extensions.Logging;
using ResultAggregation.Domain.Interfaces;
using ResultAggregation.Infrastructure.Cache;
using Shared.Events;

namespace ResultAggregation.Application.Consumers;

public class PollClosedEventConsumer : IConsumer<PollClosedEvent>
{
    private readonly RedisCacheService _cache;
    private readonly ILogger<PollClosedEventConsumer> _logger;
    private readonly IPollResultRepository _pollResultRepository;
    
    /*
    those request clients are working sync with 30s delay by default 
    better approach is using async with publishing events but for that
    I had to implement new NotificationMicroservice (maybe in future)
    */
    private readonly IRequestClient<GetVotersRequest> _getVotersClient;
    private readonly IRequestClient<GetUserEmailRequest> _getUserEmailClient;

    public PollClosedEventConsumer(
        RedisCacheService cache,
        ILogger<PollClosedEventConsumer> logger,
        IRequestClient<GetVotersRequest> getVotersClient,
        IRequestClient<GetUserEmailRequest> getUserEmailClient,
        IPollResultRepository pollResultRepository)
    {
        _cache = cache;
        _logger = logger;
        _getVotersClient = getVotersClient;
        _getUserEmailClient = getUserEmailClient;
        _pollResultRepository = pollResultRepository;
    }

    
    public async Task Consume(ConsumeContext<PollClosedEvent> context)
    {
        _logger.LogInformation("PollClosedEvent Consumed");
        
        bool anyVotes = await ComputeResults(context);
        
        if(anyVotes)
            await SendNotification(context); 
    }

    private async Task<bool> ComputeResults(ConsumeContext<PollClosedEvent> context)
    {
        var pollResult = context.Message.PollResult;
        var pollId = pollResult.PollId;
        
        _logger.LogInformation($"Computing results for closed poll {pollId}");
        double totalVotes = pollResult.Options.Sum(o => o.VoteCount);
        
        if (totalVotes == 0)
            _logger.LogWarning($"Poll {pollId} has 0 votes. Setting all percentages to 0.");
        
        foreach (var option in pollResult.Options)
        {
            if (totalVotes != 0)
            {
                double percentage = option.VoteCount * 100 / totalVotes;
                option.Percentage = Math.Round(percentage, 1);
            }
            else
            {
                option.Percentage = 0;   
            }
        }
        
        await _pollResultRepository.SetPollResultAsync(pollId, pollResult);
        _logger.LogInformation($"Cached result for poll {pollId}");
        
        return totalVotes != 0;
    }
    private async Task SendNotification(ConsumeContext<PollClosedEvent> context)
    {
        var pollResult = context.Message.PollResult;
        var pollId = pollResult.PollId;

        _logger.LogInformation($"Fetching voters for poll {pollId}");

        // get voters 
        var votersResponse = await _getVotersClient.GetResponse<GetVotersResponse>(
            new GetVotersRequest(pollId));

        var userIds = votersResponse.Message.UserIds;
        if (userIds.Count == 0)
        {
            _logger.LogWarning($"No voters found for poll {pollId}");
            return;
        }

        var emails = new List<string>();
        var missingUserIds = new List<int>();

        // Parallelism, we send all userId request Tasks independently and then wait till all finished
        // faster than when in every foreach loop send one Task request => which will be sequential
        var tasks = userIds.Select(async userId =>
        {
            var cachedEmail = await _cache.GetUserEmailAsync(userId);
            return (userId, cachedEmail);
        });

        var results = await Task.WhenAll(tasks);

        foreach (var (userId, cachedEmail) in results)
        {
            if (!string.IsNullOrEmpty(cachedEmail))
                emails.Add(cachedEmail);
            else
                missingUserIds.Add(userId);
        }

        // Request AuthService for missing emails if there is missing
        if (missingUserIds.Count > 0)
        {
            var emailResponse = await _getUserEmailClient.GetResponse<GetUserEmailResponse>
                (new GetUserEmailRequest(missingUserIds));

            foreach (var pair in emailResponse.Message.Emails)
            {
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    emails.Add(pair.Value);
                    // cache for future
                    await _cache.SetUserEmailAsync(pair.Key, pair.Value, TimeSpan.FromHours(12));
                }
            }
        }

        if (emails.Count > 0)
        {
            // ideally you need to send it to the NotificationMicroservice, but we don't have it so we will mock it with loggers
            
            _logger.LogInformation($"Sending emails for the poll {pollId}");

            foreach (var email in emails)
            {
                _logger.LogInformation
                    ($"Sending email to {email}: Hey Poll with Id {pollId} and with Title {pollResult.Title} is closed you can now see the results");
            }
        
            _logger.LogInformation($"Successfully sent notifications for poll {pollId}"); 
        }
        else
        {
            _logger.LogWarning($"No emails to notify for poll {pollId}");
        }
    }

}