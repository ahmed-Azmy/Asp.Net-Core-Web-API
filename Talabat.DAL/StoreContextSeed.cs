using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.DAL
{
    public class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext context , ILoggerFactory loggerFactory)
        {
            try
            {
                if(!context.ProductTypes.Any())
                {
                    var ProductTypesData = File.ReadAllText("../Talabat.DAL/Data/SeedData/types.json");
                    var ProductTypes = JsonSerializer.Deserialize<List<ProductType>>(ProductTypesData);
                    foreach (var ProductType in ProductTypes)
                        context.ProductTypes.Add(ProductType);
                    await context.SaveChangesAsync();
                }
                if (!context.ProductBrands.Any())
                {
                    var ProductBrandsData = File.ReadAllText("../Talabat.DAL/Data/SeedData/brands.json");
                    var ProductBrands = JsonSerializer.Deserialize<List<ProductBrand>>(ProductBrandsData);
                    foreach (var ProductBrand in ProductBrands)
                        context.ProductBrands.Add(ProductBrand);
                    await context.SaveChangesAsync();
                }
                if (!context.Products.Any())
                {
                    var ProductsData = File.ReadAllText("../Talabat.DAL/Data/SeedData/products.json");
                    var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                    foreach (var Product in Products)
                        context.Products.Add(Product);
                    await context.SaveChangesAsync();
                }
                if (!context.DeliveryMethods.Any())
                {
                    var DeliveryMethodsData = File.ReadAllText("../Talabat.DAL/Data/SeedData/delivery.json");
                    var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                    foreach (var DeliveryMethod in DeliveryMethods)
                        context.DeliveryMethods.Add(DeliveryMethod);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var Logger = loggerFactory.CreateLogger<StoreContextSeed>();
                Logger.LogError(ex, ex.Message);
            }
        }
    }
}
