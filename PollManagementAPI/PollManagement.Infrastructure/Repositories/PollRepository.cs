using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PollManagement.Domain.Entities;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;
using PollManagement.Domain.Specifications.PollSpecifications;
using PollManagement.Infrastructure.DataBases;
using Shared.Specifications;

namespace PollManagement.Infrastructure.Repositories;

public class PollRepository : IPollRepository
{
    private readonly VotePollDbContext _context;

    public PollRepository(VotePollDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Poll>> GetAllAsync(Specification<Poll> specification, bool includeVotes = false, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Polls.AsQueryable().AsNoTracking();
        
        query = query.Include(p=> p.Options);

        if(includeVotes)
            query = query.Include(p=> p.Votes);
        
        var spec = specification.ToExpression();
        query = query.Where(spec);
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return items;
    }
    public async Task<Poll?> GetByIdAsync(int id, bool includeVotes = false)
    {
        var query = _context.Polls.AsQueryable().AsNoTracking();
        query = query.Include(p=>p.Options);

        if (includeVotes)
            query = query.Include(p => p.Votes);
        
        return await query.FirstOrDefaultAsync(p => p.PollId == id);
    }
    
    public async Task<IEnumerable<Poll>> GetMyPollsAsync(int userId, bool includeVotes = false, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Polls.AsQueryable().AsNoTracking();
        query = query.Include(p => p.Options);

        if(includeVotes)
            query = query.Include(p => p.Votes);

        var spec = new PollByUserSpec(userId).ToExpression();
        query = query.Where(spec);
        
        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task AddAsync(Poll poll)
    {
        await _context.Polls.AddAsync(poll);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Poll poll)
    {
        _context.Polls.Update(poll);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Poll poll)
    {
        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync();
    }
}