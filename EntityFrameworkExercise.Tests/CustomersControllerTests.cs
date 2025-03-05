using EntityFrameworkExercise.Controllers;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using EntityFrameworkExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EntityFrameworkExercise.Tests;

public class CustomersControllerTests
{
    [Fact]
    public async Task GetResultOk()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.List(It.IsAny<CustomerSearch>()))
            .ReturnsAsync(new CustomerListResult([]));

        var controller = new CustomersController(service.Object);

        var response = await controller.Get(new CustomerQueryRequest());
        Assert.IsType<OkObjectResult>(response);
    }

    [Fact]
    public async Task GetByIdResultOk()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.Exists(It.IsAny<int>()))
           .ReturnsAsync(true);

        service.Setup(x => x.Get(It.IsAny<int>()))
            .ReturnsAsync(new CustomerResult());

        var controller = new CustomersController(service.Object);
        var response = await controller.Get(It.IsAny<int>());

        Assert.IsType<OkObjectResult>(response);
        service.Verify(x => x.Get(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdResultNotFound()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.Exists(It.IsAny<int>()))
           .ReturnsAsync(false);

        var controller = new CustomersController(service.Object);
        var response = await controller.Get(It.IsAny<int>());

        Assert.IsType<NotFoundResult>(response);
        service.Verify(x => x.Get(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task PutNoContent()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.Exists(It.IsAny<int>()))
           .ReturnsAsync(true);
        var controller = new CustomersController(service.Object);

        var response = await controller.Put(It.IsAny<int>(), new());

        Assert.IsType<NoContentResult>(response);
        service.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task PutNotFoundt()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.Exists(It.IsAny<int>()))
           .ReturnsAsync(false);

        var controller = new CustomersController(service.Object);

        var response = await controller.Put(It.IsAny<int>(), new());

        Assert.IsType<NotFoundResult>(response);
        service.Verify(x => x.Update(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task PostCreatedAtAction()
    {
        var service = new Mock<ICustomerService>();
        var controller = new CustomersController(service.Object);

        var response = await controller.Post(new());

        var result = Assert.IsType<CreatedAtActionResult>(response);
        service.Verify(x => x.Create(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOk()
    {
        var service = new Mock<ICustomerService>();
        var controller = new CustomersController(service.Object);

        var response = await controller.Delete(It.IsAny<int>());
        service.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
    }
}