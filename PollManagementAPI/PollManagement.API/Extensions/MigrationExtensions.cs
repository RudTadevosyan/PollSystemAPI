using Microsoft.EntityFrameworkCore;
using PollManagement.Infrastructure.DataBases;

namespace PollManagement.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
        using VotePollDbContext db = scope.ServiceProvider.GetRequiredService<VotePollDbContext>();
        
        db.Database.Migrate();
    }
}