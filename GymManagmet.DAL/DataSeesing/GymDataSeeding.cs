using GymManagmet.DbContexts;
using GymManagmet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GymManagmet.DAL.DataSeesing
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext,string seedFilesPath,ILogger logger,CancellationToken ct = default)
        {
            try
            {
                if (!await dbContext.Plans.AnyAsync(ct))
                {
                    var plans = LoadDataFromJsonFile<Plan>(seedFilesPath, "plans.json");
                    if (plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation("Seeded {Count} plans.", plans.Count);
                    }
                }

                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync(ct);
                else
                    logger.LogInformation("Plan already seeded");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gym data seeding failed.");
                throw;
            }
        }

        private static List<T> LoadDataFromJsonFile<T>( string FolderPath , string fileName)
        {

            var filePath = Path.Combine(FolderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed data file not found: {filePath}");

            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
