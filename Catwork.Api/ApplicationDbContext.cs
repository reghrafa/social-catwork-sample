using Catwork.Api.DbModels;
using Microsoft.EntityFrameworkCore;
namespace Catwork.Api
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CatworkUser> CatworkUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }   
}
