using System.Linq.Expressions;
using EntityFrameworkExercise.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkExercise.Data;

public class StoreContext(DbContextOptions<StoreContext> options)
    : DbContext(options), IStoreContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("store");
        modelBuilder.Entity<Product>()
            .HasMany(x => x.Sales)
            .WithMany(x => x.Products)
            .UsingEntity<ProductSale>();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Seller> Sellers { get; set; } = default!;
    public DbSet<Sale> Sales { get; set; } = default!;
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<ProductSale> ProductsSales { get; set; } = default!;

    public Expression<Func<Customer, bool>> SearhByTerm(string? searchTerm)
    {
        return c => searchTerm == null || EF.Functions.Like(c.Name.ToLower(), $"%{searchTerm.ToLower()}%");
    }

    public async Task ExecuteUpdate<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls)
        where TEntity : class
    {
        await Set<TEntity>()
            .Where(predicate)
            .ExecuteUpdateAsync(setPropertyCalls);
    }
}