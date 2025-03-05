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
    public async Task ShoudGetList()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var customers = Enumerable
            .Range(0, Random.Shared.Next())
            .Select(r => new Customer
            {
                Id = r,
                Name = Guid.NewGuid().ToString(),
                Sales = [],
            });

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        context.Setup(x => x.SearhByTerm(It.IsAny<string>()))
            .Returns((Customer customer) => true);
        var service = new CustomerService(context.Object);
        var result = await service.List(new());

        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ShoudGetListFilterByPageSize()
    {
        var sizeByPage = 5;

        var customers = Enumerable
            .Range(0, Random.Shared.Next(10, 20))
            .Select(r => new Customer
            {
                Id = r,
                Name = Guid.NewGuid().ToString(),
                Sales = [],
            });

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet(customers);
        context.Setup(x => x.SearhByTerm(It.IsAny<string>()))
            .Returns((Customer customer) => true);

        var service = new CustomerService(context.Object);

        var searsh = new CustomerSearch
        {
            Page = 2,
            Size = sizeByPage,
        };

        var result = await service.List(searsh);

        Assert.Equal(sizeByPage, result.Count);
    }

    [Fact]
    public async Task ShoudGetListFilterByTermWhenFound()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        context.Setup(x => x.SearhByTerm(It.Is<string>(term => customer.Name.Contains(term))))
            .Returns((Customer customer) => true);

        var service = new CustomerService(context.Object);

        var search = new CustomerSearch
        {
            Term = customer.Name[Random.Shared.Next(0, customer.Name.Length)..^1]
        };

        var result = await service.List(search);

        Assert.Single(result);
    }

    [Fact]
    public async Task ShoudGetListFilterByTermWhenNotFound()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        context.Setup(x => x.SearhByTerm(It.Is<string>(term => customer.Name.Contains(term))))
            .Returns((Customer customer) => false);

        var service = new CustomerService(context.Object);

        var search = new CustomerSearch
        {
            Term = customer.Name[Random.Shared.Next(0, customer.Name.Length)..^1]
        };

        var result = await service.List(search);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ShoudGetSingle()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        var service = new CustomerService(context.Object);
        var result = await service.Get(customer.Id);

        Assert.Equal(result.Name, customer.Name);
    }

    [Fact]
    public async Task ShoudThrowsWhenGetByIdNotFound()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        var service = new CustomerService(context.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Get(It.IsAny<int>()));
        Assert.Equal("Sequence contains no elements", ex.Message);
    }

    [Fact]
    public async Task ShoudThrowsWhenGetByIdFoundMany()
    {
        var customer1 = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var customer2 = new Customer
        {
            Id = customer1.Id,
            Name = Guid.NewGuid().ToString(),
            Sales = [],
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer1, customer2]);

        var service = new CustomerService(context.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Get(customer1.Id));
        Assert.Equal("Sequence contains more than one element", ex.Message);
    }

    [Fact]
    public async Task ShoudGetSingleExactlySalesCount()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
            Name = Guid.NewGuid().ToString(),
            Sales = Enumerable.Range(0, Random.Shared.Next(100))
            .Select(r => new Sale { Id = r })
            .ToList(),
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        var service = new CustomerService(context.Object);
        var result = await service.Get(customer.Id);

        Assert.Equal(result.Sales.Count(), customer.Sales.Count);
    }

    [Fact]
    public async Task ShouldExists()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        var service = new CustomerService(context.Object);
        var result = await service.Exists(customer.Id);
        Assert.True(result);
    }

    [Fact]
    public async Task ShouldNotExists()
    {
        var customer = new Customer
        {
            Id = Random.Shared.Next(),
        };

        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([customer]);

        var service = new CustomerService(context.Object);
        var result = await service.Exists(It.IsAny<int>());
        Assert.False(result);
    }

    [Fact]
    public async Task ShouldCreate()
    {
        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([]);

        var service = new CustomerService(context.Object);
        var customer = new Customer
        {
            Name = Guid.NewGuid().ToString(),
        };
        await service.Create(customer);
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldUpdate()
    {
        var context = new Mock<IStoreContext>();

        context.Setup(x => x.Customers)
            .ReturnsDbSet([]);

        var service = new CustomerService(context.Object);
        var customer = new Customer
        {
            Name = Guid.NewGuid().ToString(),
        };

        await service.Update(customer);

        context.Verify(x => x.ExecuteUpdate(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<SetPropertyCalls<Customer>, SetPropertyCalls<Customer>>>>()), Times.Once);
    }
}