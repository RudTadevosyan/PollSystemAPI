using System.Net.Http.Json;
using ResultAggregation.Domain.Interfaces;
using Shared.DTOs.Poll;

namespace ResultAggregation.Application.Services;

public class PollResultService : IPollResultService
{
    private readonly IPollResultRepository _pollResultRepository;
    private readonly HttpClient _httpClient;

    public PollResultService(IHttpClientFactory httpClient, IPollResultRepository pollResultRepository)
    {
        _pollResultRepository = pollResultRepository;
        _httpClient = httpClient.CreateClient("PollManagement");
    }

    public async Task<PollResultDto?> GetPollResult(int pollId)
    {
        //this will return true or false 
        var response = await _httpClient.GetAsync($"api/polls/{pollId}/status");
        if (!response.IsSuccessStatusCode)
        {
          await _pollResultRepository.DeletePollResultAsync(pollId);
          return null;
        }
        
        var status = await response.Content.ReadFromJsonAsync<Boolean>();
        if (status)
          return null;
        
        return await _pollResultRepository.GetPollResultAsync<PollResultDto>(pollId);
    }
}