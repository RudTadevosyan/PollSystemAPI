    using Microsoft.EntityFrameworkCore;
    using PollManagement.Domain.Entities;
    using PollManagement.Domain.Interfaces.RepositoryInterfaces;
    using PollManagement.Infrastructure.DataBases;

    namespace PollManagement.Infrastructure.Repositories;

    public class OptionsRepository : IOptionRepository
    {
        private readonly VotePollDbContext _context;

        public OptionsRepository(VotePollDbContext context)
        {
            _context = context;
        }
        
        public async Task<Option?> GetByIdAsync(int optionId, bool trackChanges = false)
        {
            IQueryable<Option> option = _context.Options;

            if (!trackChanges)
                option = option.AsNoTracking();
            
            return await option
                .Include(o => o.Poll)
                .FirstOrDefaultAsync(o => o.OptionId == optionId);
        }
        
        public async Task AddAsync(Option option)
        {
            await _context.Options.AddAsync(option);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Option option)
        {
            _context.Options.Update(option);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Option option)
        {
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
        }
    }