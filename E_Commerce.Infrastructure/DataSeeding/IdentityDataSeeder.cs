using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<IdentityDataSeeder> logger;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext , 
            UserManager<ApplicationUser> userManager , 
            RoleManager<IdentityRole> roleManager , 
            ILogger<IdentityDataSeeder> logger)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                if (!await roleManager.Roles.AnyAsync())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }

                if (!await userManager.Users.AnyAsync())
                {
                    var admin = new ApplicationUser()
                    {
                        DisplayName = "Zyad Emad",
                        Email = "zyad.emad19@gmail.com",
                        UserName = "ZyadEmad",
                        PhoneNumber = "01018946895"
                    };

                    var createRes = await userManager.CreateAsync(admin, "P@ssw0rd");
                    if (createRes.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "SuperAdmin");
                    }
                    else
                    {
                        var Errors = string.Join(' ', createRes.Errors.Select(e => e.Description));
                        logger.LogWarning($"Can Not Seed Default Admin {Errors}");
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Identity Data Seeding Failed ");
                return;
            }
        }
    }
}
