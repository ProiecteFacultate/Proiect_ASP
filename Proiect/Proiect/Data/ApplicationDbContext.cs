using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proiect.Models;

namespace Proiect.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Post_Comment> Post_Comments { get; set; }
    }
}