using GymManagmet.Configurations;
using GymManagmet.Models;
using Microsoft.EntityFrameworkCore;
namespace GymManagmet.DbContexts
{
    public class GymDbContext : DbContext
    {
      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=GymDb;Trusted_Connection=True; TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfiguration());

        }

        public DbSet<Plan> Plans { get; set; } = default!;





    }
}