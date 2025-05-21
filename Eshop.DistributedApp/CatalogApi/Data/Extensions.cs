namespace CatalogApi.Data;

public static class Extensions
{
    public static void UseMigration(this WebApplication app)
    {
       using var scope = app.Services.CreateScope();
       
        var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        context.Database.Migrate();
        DatabaseSeeder.Seed(context);
    }
}

public class DatabaseSeeder
{
    public static void Seed(ProductDbContext context)
    {
        if (context.Products.Any())
            return;
        context.Products.AddRange(Products);
        context.SaveChanges();
    }

    public static IEnumerable<Product> Products =>
    [
        new Product { Name="Solar Powered FlashLight", Description="A fantastic Flashlight with Solar powered.", Price=30, ImageUrl="https://m.media-amazon.com/images/I/61FOUArF3nL._AC_UL320_.jpg"},
        new Product { Name="Wireless Earbuds", Description="Bluetooth 5.0 earbuds with noise cancellation.", Price=49.99m, ImageUrl="https://m.media-amazon.com/images/I/71zny7BTRlL._AC_UL320_.jpg"},
        new Product { Name="Smart Watch", Description="Fitness tracker with heart rate monitor and GPS.", Price=89.99m, ImageUrl="https://m.media-amazon.com/images/I/714JxCc-nZL._AC_UY218_.jpg"},
        new Product { Name="Portable Power Bank", Description="20000mAh fast charging power bank with dual USB ports.", Price=35.50m, ImageUrl="https://m.media-amazon.com/images/I/71UyNLSv2mL._AC_UL320_.jpg"},
        new Product { Name="Bluetooth Speaker", Description="Waterproof portable speaker with 24-hour battery life.", Price=59.99m, ImageUrl="https://m.media-amazon.com/images/I/71JB6hM6Z6L._AC_UL320_.jpg"},
        new Product { Name="Laptop Backpack", Description="Anti-theft backpack with USB charging port and laptop compartment.", Price=42.99m, ImageUrl="https://m.media-amazon.com/images/I/81idlqFqcUL._AC_UY218_.jpg"},
        new Product { Name="Wireless Mouse", Description="Ergonomic wireless mouse with adjustable DPI settings.", Price=24.99m, ImageUrl="https://m.media-amazon.com/images/I/61TepVypbSL._AC_UY218_.jpg"}
    ];
}
