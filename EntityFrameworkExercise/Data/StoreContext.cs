using System.Linq.Expressions;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Services;
using Microsoft.EntityFrameworkCore;

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

    public Expression<Func<Customer, bool>> SearchCustomerName(CustomerSearch search)
    {
        return c => search.Term == null
                                  || EF.Functions.Like(c.Name.ToLower(), $"%{search.Term.ToLower()}%");
    }
}

public interface IStoreContext
{
    DbSet<Seller> Sellers { get; set; }
    DbSet<Sale> Sales { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ProductSale> ProductsSales { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Expression<Func<Customer, bool>> SearchCustomerName(CustomerSearch search);
}