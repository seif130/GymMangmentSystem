using GymManagmet.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.DataSeesing
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager,
                                                  UserManager<ApplicationUser> userManager, ILogger logger,
            CancellationToken ct = default)
        {

            try
            {
                bool HasUsers = userManager.Users.Any();
                bool HasRoles = roleManager.Roles.Any();

                if (HasUsers && HasRoles) return;
                if (!HasRoles)
                {
                    var Roles = new List<IdentityRole>()
                    {
                        new IdentityRole(){Name = "SuperAdmin"},
                        new IdentityRole(){Name = "Admin"}
                    };

                    foreach (var role in Roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            var roleResult = await roleManager.CreateAsync(role);
                            if (!roleResult.Succeeded)
                                logger.LogError($"Failed to create role {role.Name}" );
                        }
                    }
                }
                if (!HasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Seif",
                        LastName = "Amr",
                        UserName = "seifamr",
                        Email = "seif@gmail.com",
                        PhoneNumber = "01123652600"
                    };

                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin01 = new ApplicationUser()
                    {
                        FirstName = "mostafa",
                        LastName = "Mohamed",
                        UserName = "MostafaMohamed",
                        Email = "mostafa@gmail.com",
                        PhoneNumber = "01232589652"
                    };

                    var createResult = await userManager.CreateAsync(Admin01, "P@ssw0rd");
                    if (!createResult.Succeeded)
                    {
                        logger.LogError("Failed to create seed SuperAdmin: {Errors}", string.Join("; ", createResult.Errors.Select(e => e.Description)));
                        return;
                    }
                    logger.LogInformation($"Seeded SuperAdmin {Admin01.Email}");

                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity seeding failed.");
                throw;
            }
        }


    }
    }

