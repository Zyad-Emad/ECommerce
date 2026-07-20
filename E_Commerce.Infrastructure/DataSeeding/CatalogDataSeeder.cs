using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext , ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (PendingMigrations.Any())
                {
                   await dbContext.Database.MigrateAsync(ct);
                }
                var seedRoot = Path.Combine(AppContext.BaseDirectory, "DataSeed");

                await SeedIfEmptyAsync<ProductBrand , int>(seedRoot, "brands.json" , ct);
                await SeedIfEmptyAsync<ProductType , int>(seedRoot, "types.json" , ct);
                await SeedIfEmptyAsync<Product , int>(seedRoot, "products.json" , ct);
                await SeedIfEmptyAsync<DeliveryMethod , int>(seedRoot, "delivery.json", ct);

                int res = await dbContext.SaveChangesAsync(ct);

                if (res > 0)
                    logger.LogInformation($"{res} Rows Added");
                else
                    logger.LogInformation("Database Already Seeded");
            }
            catch
            {

            }
        }
        private async Task SeedIfEmptyAsync<T , TKey>(string rootPath , string fileName , CancellationToken ct) where T : BaseEntity<TKey>
        {
            if (await dbContext.Set<T>().AnyAsync())
            {
                logger.LogInformation("Table Already Has Data");
                return;
            }

            var filePath = Path.Combine(rootPath, fileName);
            if (!File.Exists(filePath))
            {
                logger.LogWarning($"File {fileName} Is Not Found");
                return;
            }
            using var fileStream = File.OpenRead(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            var items = await JsonSerializer.DeserializeAsync<List<T>>(fileStream , options , ct);

            if (items?.Any() ?? false)
            {
                dbContext.Set<T>().AddRange(items);
            }



            

        }
    }
}
