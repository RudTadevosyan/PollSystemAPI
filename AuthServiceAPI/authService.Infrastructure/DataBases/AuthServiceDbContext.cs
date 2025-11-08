using authService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace authService.Infrastructure.DataBases
{
    public class AuthServiceDbContext : IdentityDbContext<User, IdentityRole<int>, int> //int => TKey 
    {
        public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : base(options) { }

    }
}
