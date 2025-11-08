using PollManagement.Domain.Entities;

namespace PollManagement.Domain.Interfaces.RepositoryInterfaces;

public interface IOptionRepository
{
    Task<Option?> GetByIdAsync(int optionId, bool trackChanges = false);
    Task AddAsync(Option option);
    Task UpdateAsync(Option option);
    Task DeleteAsync(Option option);
}