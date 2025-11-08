using authService.Infrastructure.DataBases;
using Microsoft.EntityFrameworkCore;

namespace authService.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
        using AuthServiceDbContext db = scope.ServiceProvider.GetRequiredService<AuthServiceDbContext>();
        
        db.Database.Migrate();
    }
}