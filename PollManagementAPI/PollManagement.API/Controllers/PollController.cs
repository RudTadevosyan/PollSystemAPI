using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollManagement.API.Attributes;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using Shared.DTOs.Poll;
using PollManagement.API.Extensions;

namespace PollManagement.API.Controllers;

[ApiController]
[Route("api/polls")]
public class PollController : ControllerBase
{
    private readonly IPollService _pollService;

    public PollController(IPollService pollService)
    {
        _pollService = pollService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllPolls([FromQuery] PollFilterDto pollFilterDto, bool includeVotes = false, int pageNumber = 1, int pageSize = 10)
    {
        var polls = await _pollService.GetAllPolls(pollFilterDto, includeVotes, pageNumber, pageSize);
        return Ok(polls);
    }

    [HttpGet("{pollId:int}")]
    public async Task<IActionResult> GetPollById(int pollId, bool includeVotes = false)
    {
        var poll = await _pollService.GetPollByIdAsync(pollId, includeVotes);
        return Ok(poll);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyPolls(bool includeVotes = false, int pageNumber = 1, int pageSize = 10)
    {
        var userId = User.GetUserId();
        var poll = await _pollService.GetMyPollsAsync(userId, includeVotes, pageNumber, pageSize);
        return Ok(poll);
    }

    [HttpGet("{pollId:int}/status")]
    public async Task<IActionResult> GetPollStatus(int pollId)
    {
        var poll = await _pollService.GetPollByIdAsync(pollId);
        return Ok(poll?.Status);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePoll([FromBody] CreatePollDto createPollDto)
    {
        var userId = User.GetUserId();
        
        var poll = await _pollService.CreatePollAsync(createPollDto, userId);
        
        return Ok(poll);
    }
    
    [Authorize]
    [CheckPollStatus]
    [HttpPut("{pollId:int}")]
    public async Task<IActionResult> UpdatePoll(int pollId, [FromBody] UpdatePollDto updatePollDto)
    {
        var userId = User.GetUserId();

        await _pollService.UpdatePollAsync(updatePollDto, pollId, userId);
        return Ok();
    }

    [Authorize]
    [HttpDelete("{pollId:int}")]
    public async Task<IActionResult> DeletePoll(int pollId)
    {
        var userId = User.GetUserId();
        
        await _pollService.DeletePollAsync(pollId, userId);
        return Ok();
    }
    
    // Could be deleted this is testing for jwt header
    [HttpPost("test")]
    public IActionResult TestUser()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
}