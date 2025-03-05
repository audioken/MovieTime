using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MowiTajm.Models;

namespace MowiTajm.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Lagt till ApplicationUser.
    {
        public DbSet<Review> Reviews { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
