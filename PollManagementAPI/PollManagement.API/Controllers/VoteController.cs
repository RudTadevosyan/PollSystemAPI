using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollManagement.API.Attributes;
using PollManagement.API.Extensions;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using Shared.DTOs.Vote;

namespace PollManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/polls/{pollId:int}/vote")]
public class VoteController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpGet("/api/votes/mine")]
    public async Task<IActionResult> GetMyVotes(int pageNumber = 1, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var votes = await _voteService.GetMyVotesAsync(userId,  pageNumber, pageSize);
        return Ok(votes);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyVote(int pollId)
    {
        var userId= User.GetUserId();
        var vote = await _voteService.GetVoteByIdAsync(pollId, userId);
        return Ok(vote);
    }

    [HttpPost]
    [CheckPollStatus]
    public async Task<IActionResult> AddVote(int pollId, [FromBody] CreateVoteDto voteDto)
    {
        var userId = User.GetUserId();
        var vote = await _voteService.AddVoteAsync(voteDto, pollId, userId);
        return Ok(vote);
    }

    [HttpDelete]
    [CheckPollStatus]
    public async Task<IActionResult> DeleteVote(int pollId)
    {
        var userId = User.GetUserId();
        await _voteService.DeleteVoteAsync(pollId, userId);
        return Ok();
    }
}