using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Services;
using Moq;
using Moq.EntityFrameworkCore;

namespace EntityFrameworkExercise.Tests;

public class CustomerServiceTests
{
    [Fact]
    public async Task List() 
    {
        var customers = new List<Customer>
        {
           new ()
           {
               Id = Random.Shared.Next(),
               Name = Guid.NewGuid().ToString(),
               Sales =[],
           },
           new ()
           {
               Id = Random.Shared.Next(),
               Name = Guid.NewGuid().ToString(),
               Sales =[],
           },
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet(customers);

        var service = new CustomerService(context.Object);

        var search = new CustomerSearch 
        {
            Term = customers[0].Name[^5..^1],
        };
        var result = await service.List(search);

        Assert.Single(result);
    }
    
    [Fact]
    public async Task Get()
    {
        var customers = new List<Customer>
        {
           new ()
           {
               Id = Random.Shared.Next(),
               Name = Guid.NewGuid().ToString(),
               Sales =[],
           },
           new ()
           {
               Id = Random.Shared.Next(),
               Name = Guid.NewGuid().ToString(),
               Sales =[],
           },
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet(customers);

        var service = new CustomerService(context.Object);

        var customer = await service.Get(customers[0].Id);

        Assert.Equal(customers[0].Name, customer.Name);
    }

    [Fact]
    public async Task Exists()
    {
        var customer = new Customer()
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        var service = new CustomerService(context.Object);

        var exists = await service.Exists(customer.Id);

        Assert.True(exists);
    }
}