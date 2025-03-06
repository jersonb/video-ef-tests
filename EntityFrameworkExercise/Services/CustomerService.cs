using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkExercise.Services;

public class CustomerService(IStoreContext context) : ICustomerService
{
    public async Task<CustomerListResult> List(CustomerSearch search)
    {
        var query = context.Customers
               .Where(context.SearchCustomerName(search))
               .OrderBy(c => c.Id)
               .Skip((search.Page - 1) * search.Size)
               .Take(search.Size)
               .Select(c => new CustomerResult
               {
                   Id = c.Id,
                   Name = c.Name,
                   SalesCount = c.Sales.Count
               });

        var customers = await query.ToListAsync();

        return new(customers);
    }

    public async Task<CustomerResult> Get(int id)
    {
        var query = context.Customers
             .Where(c => c.Id == id)
             .Select(c => new CustomerResult
             {
                 Id = c.Id,
                 Name = c.Name,
                 Sales = c.Sales.Select(s => new CustomerSaleResult(s.Id, s.Date))
             });

        var customer = await query.SingleAsync();
        return customer;
    }

    public async Task<bool> Exists(int id)
    {
        return await context.Customers.AnyAsync(c => c.Id == id);
    }

    public async Task<int> Create(Customer customer)
    {
        Validate(customer);

        context.Customers.Add(customer);

        await context.SaveChangesAsync();
        return customer.Id;
    }

    public async Task Update(Customer customer)
    {
        Validate(customer);

        await context.Customers
            .Where(c => c.Id == customer.Id)
            .ExecuteUpdateAsync(prop =>
               prop.SetProperty(c => c.Name, customer.Name));
    }

    public async Task Delete(int id)
    {
        await context.Customers
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
    }

    private static void Validate(Customer customer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customer.Name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(customer.Name.Length, 3);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(customer.Name.Length, 150);
    }
}

public interface ICustomerService
{
    Task<bool> Exists(int id);

    Task<CustomerResult> Get(int id);

    Task<CustomerListResult> List(CustomerSearch search);

    Task<int> Create(Customer customer);

    Task Update(Customer customer);

    Task Delete(int id);
}

public class CustomerSearch
{
    public int Page { get; set; } = 1;

    public int Size { get; set; } = 5;

    public string? Term { get; set; } = null;
}

public class CustomerListResult(IEnumerable<CustomerResult> collection)
    : List<CustomerResult>(collection);

public record CustomerResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<CustomerSaleResult> Sales { get; set; } = [];
    public int SalesCount { get; set; }
}

public record CustomerSaleResult(int Id, DateTimeOffset Date);