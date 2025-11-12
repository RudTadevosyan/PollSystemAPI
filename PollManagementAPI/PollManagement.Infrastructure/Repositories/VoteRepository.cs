using Microsoft.EntityFrameworkCore;
using PollManagement.Domain.Entities;
using PollManagement.Domain.Exceptions;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;
using PollManagement.Infrastructure.DataBases;

namespace PollManagement.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly VotePollDbContext _context;
    
    public VoteRepository(VotePollDbContext context)
    {
        _context = context;
    }

    public async Task<Vote?> GetByUserAndPollAsync(int pollId, int userId)
    {
        return await _context.Votes.AsNoTracking()
            .Include(vote => vote.Poll)
            .Include(vote => vote.Option)
            .FirstOrDefaultAsync(v => v.UserId == userId && v.PollId == pollId);
    }

    public async Task<IEnumerable<Vote>> GetMyVotesAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Votes
            .AsNoTracking()
            .Where(v => v.UserId == userId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Safe Voting => Atomicity
    public async Task AddAsync(Vote vote)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var poll = await _context.Polls.FindAsync(vote.PollId)
                ?? throw new NotFoundException("Poll not found.");
            
            if (poll.ClosesAt < DateTime.UtcNow || !poll.Status)
                throw new PollClosedException("The poll is closed.");
            
            var alreadyVoted = await _context.Votes
                .AnyAsync(v => v.UserId == vote.UserId && v.PollId == vote.PollId);

            if (alreadyVoted)
                throw new DomainException("User already voted in this poll.");
            
            var optionExists = await _context.Options
                .AnyAsync(o => o.OptionId == vote.OptionId && o.PollId == vote.PollId);

            if (!optionExists)
                throw new DomainException("Invalid option for this poll.");

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"Options\" SET \"VoteCount\" = \"VoteCount\" + 1 WHERE \"OptionId\" = {vote.OptionId}");
            
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch   
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Safe Deleting => Atomicity
    public async Task DeleteAsync(Vote vote)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"Options\" SET \"VoteCount\" = \"VoteCount\" - 1 WHERE \"OptionId\" = {vote.OptionId}");
            
            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}