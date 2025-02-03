using Microsoft.EntityFrameworkCore;

namespace TransactionsDemo;

internal class WithEntityFramework
{
    public static async Task Run()
    {
        await using var context = new AppDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Products.Add(new Product { Name = "Product1", Price = 100 });
            await context.SaveChangesAsync();

            //savepoint
            await transaction.CreateSavepointAsync("name");

            context.Products.Add(new Product { Name = "Product2", Price = 200 });
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }
    }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
}

public class Product
{
    public string Name { get; set; }
    public int Price { get; set; }
}