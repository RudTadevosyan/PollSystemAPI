using Microsoft.AspNetCore.Mvc;
using ResultAggregation.Domain.Interfaces;

namespace ResultAggregation.API.Controllers;

[ApiController]
[Route("api/polls")]
public class PollController : ControllerBase
{
    private readonly IPollResultService _pollResultService;

    public PollController(IPollResultService pollResultService)
    {
        _pollResultService = pollResultService;
    }

    [HttpGet("{pollId:int}/result")]
    public async Task<IActionResult> GetPollResult(int pollId)
    {
        var result = await _pollResultService.GetPollResult(pollId);
        if(result == null)
            return BadRequest("Poll not found or it is not closed");
        
        return Ok(result);
    }
}