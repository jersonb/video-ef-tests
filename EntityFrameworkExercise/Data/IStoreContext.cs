using System.Linq.Expressions;
using EntityFrameworkExercise.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkExercise.Data;

public interface IStoreContext
{
    DbSet<Seller> Sellers { get; set; }
    DbSet<Sale> Sales { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ProductSale> ProductsSales { get; set; }

    Task ExecuteUpdate<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Expression<Func<Customer, bool>> SearhByTerm(string? searchTerm);
}