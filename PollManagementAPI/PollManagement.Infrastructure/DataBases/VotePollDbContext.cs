using Microsoft.EntityFrameworkCore;
using PollManagement.Domain.Entities;

namespace PollManagement.Infrastructure.DataBases;

public class VotePollDbContext : DbContext
{
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Vote> Votes { get; set; }
    
    
    public VotePollDbContext(DbContextOptions<VotePollDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    modelBuilder.Entity<Poll>(entity =>
    {
        entity.HasKey(e => e.PollId); //PK

        entity.Property(e => e.Title)
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.UserId)
              .IsRequired();

        entity.Property(e => e.ClosesAt)
              .IsRequired();

        entity.Property(e => e.Category)
              .HasMaxLength(50);

        entity.Property(e => e.Topic)
              .HasMaxLength(100);

        entity.Property(e => e.Status)
              .HasDefaultValue(true);
    });

    modelBuilder.Entity<Option>(entity =>
    {
        entity.HasKey(e => e.OptionId); // PK
        
        entity.Property(e => e.Text)
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.VoteCount)
              .HasDefaultValue(0);

        // Fk to polls 
        entity.HasOne(d => d.Poll)
              .WithMany(p => p.Options)
              .HasForeignKey(d => d.PollId)
              .OnDelete(DeleteBehavior.Cascade)
              .HasConstraintName("FK_PollOptions_Polls_PollId");
    });
    
    modelBuilder.Entity<Vote>(entity =>
    {
          //PK also ensures that one user vote only one time to the poll
        entity.HasKey(v => new { v.UserId, v.PollId }); 

        entity.Property(v => v.UserId)
              .IsRequired();

        entity.Property(v => v.VotedAt)
              .HasDefaultValueSql("timezone('utc', now())");

        //FK to Option
        entity.HasOne(v => v.Option)
              .WithMany()
              .HasForeignKey(v => v.OptionId)
              .OnDelete(DeleteBehavior.Cascade);
        
        //Fk to Poll
        entity.HasOne(v => v.Poll)
              .WithMany(p => p.Votes)
              .HasForeignKey(v => v.PollId)
              .OnDelete(DeleteBehavior.Cascade);
        
        // NON-CLUSTERED INDEX: Fast lookup by UserId 
        entity.HasIndex(v => v.UserId);

        //Non-clustered index
        entity.HasIndex(v => new { v.PollId, v.OptionId });
    });
    }
}