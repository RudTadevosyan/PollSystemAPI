using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollManagement.API.Attributes;
using PollManagement.API.Extensions;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using Shared.DTOs.Option;

namespace PollManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/polls/{pollId:int}/options")]
public class OptionController : ControllerBase
{
    private readonly IOptionService _optionService;

    public OptionController(IOptionService optionService)
    {
        _optionService = optionService;
    }

    [HttpGet("{optionId:int}")]
    public async Task<IActionResult> GetOptionById(int pollId, int optionId)
    {
        var option = await _optionService.GetOptionByIdAsync(pollId, optionId);
        return Ok(option);
    }

    [HttpPost]
    [CheckPollStatus]
    public async Task<IActionResult> AddOption(int pollId, [FromBody] CreateOptionDto optionDto)
    {
        var userId = User.GetUserId();
        var option = await _optionService.AddOptionAsync(optionDto, pollId, userId);
        return Ok(option);
    }
    
    [CheckPollStatus]
    [HttpPut("{optionId:int}")]
    public async Task<IActionResult> UpdateOption(int pollId, int optionId, [FromBody] UpdateOptionDto optionDto)
    {
        var userId = User.GetUserId();
        await _optionService.UpdateOptionAsync(optionDto, pollId, optionId, userId);
        return Ok();
    }
    
    [CheckPollStatus]
    [HttpDelete("{optionId:int}")] 
    public async Task<IActionResult> DeleteOption(int pollId, int optionId)
    {
        var userId = User.GetUserId();
        await _optionService.DeleteOptionAsync(pollId, optionId, userId);
        return Ok();
    }
}