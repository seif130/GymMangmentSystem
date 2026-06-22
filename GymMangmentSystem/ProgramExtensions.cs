using GymManagmet.DAL.DataSeesing;
using GymManagmet.DAL.Models;
using GymManagmet.DbContexts;
using GymMangmentSystem;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMangment.PL
{
    public static class ProgramExtensions
    {
        public  static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
           
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} pending migrations ." );
                await dbContext.Database.MigrateAsync();
            }

            var seedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedPath, logger);
            await IdentityDataSeeding.SeedIdentityDataAsync(roleManager,userManager,logger);
        }
    }
    }

