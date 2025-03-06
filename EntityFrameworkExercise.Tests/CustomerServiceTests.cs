using System.Linq.Expressions;
using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Services;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Moq.EntityFrameworkCore;

namespace EntityFrameworkExercise.Tests;

public class CustomerServiceTests
{
    [Fact]
    public async Task List()
    {
        var customer = new Customer()
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var searchTerm = customer.Name[^5..^1];

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        context.Setup(x => x.SearchCustomerName(It.Is<CustomerSearch>(x => x.Term!.Contains(searchTerm))))
            .Returns((Customer c) => true);

        var service = new CustomerService(context.Object);

        var search = new CustomerSearch
        {
            Term = searchTerm
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

    [Fact]
    public async Task Create()
    {
        var customer = new Customer()
        {
            Name = Guid.NewGuid().ToString(),
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([]);

        var service = new CustomerService(context.Object);

        await service.Create(customer);

        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update()
    {
        var customer = new Customer()
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([]);

        var service = new CustomerService(context.Object);

        await service.Update(customer);

        context.Verify(x => x.ExecuteUpdate(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<SetPropertyCalls<Customer>, SetPropertyCalls<Customer>>>>()), Times.Once);
    }

    [Fact]
    public async Task Delete()
    {
        var context = new Mock<IStoreContext>();
        var service = new CustomerService(context.Object);
        await service.Delete(It.IsAny<int>());

        context.Verify(x => x.ExecuteDelete(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);
    }
}