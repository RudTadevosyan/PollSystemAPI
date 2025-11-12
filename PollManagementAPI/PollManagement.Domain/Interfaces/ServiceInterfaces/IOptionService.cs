using Shared.DTOs.Option;

namespace PollManagement.Domain.Interfaces.ServiceInterfaces;

public interface IOptionService
{
    Task<OptionDto> GetOptionByIdAsync(int pollId, int optionId);
    Task<OptionDto> AddOptionAsync(CreateOptionDto optionDto, int pollId, int userId);
    Task UpdateOptionAsync(UpdateOptionDto optionDto, int pollId,  int optionId, int userId);
    Task DeleteOptionAsync(int pollId, int optionId, int userId);
}