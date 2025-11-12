using PollManagement.API.Attributes;
using PollManagement.Domain.Exceptions;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;

namespace PollManagement.API.Middlewares;

public class PollExpirationMiddleware
{
    private readonly RequestDelegate _next;
    
    public PollExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IPollRepository pollRepository)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<CheckPollStatus>() != null)
        {
            if (httpContext.Request.RouteValues.TryGetValue("pollId", out var pollIdObj)
                && int.TryParse(pollIdObj?.ToString(), out int pollId))
            {
                var poll = await pollRepository.GetByIdAsync(pollId)
                           ?? throw new NotFoundException($"Poll with id {pollId} not found");

                if (poll.ClosesAt <= DateTime.UtcNow || ! poll.Status)
                    throw new PollClosedException("This poll is closed. Voting, UnVoting or Poll modification is not allowed.");
            }
        }
        
        await _next(httpContext);
    }
}