using EntityFrameworkExercise.Controllers;
using EntityFrameworkExercise.Requests;
using EntityFrameworkExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EntityFrameworkExercise.Tests;

public class CustomersControllerTests
{
    [Fact]
    public async Task ShouldGetResultOk()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.List(It.IsAny<CustomerSearch>()))
            .ReturnsAsync(new CustomerListResult([]));

        var controller = new CustomersController(service.Object);
        var response = await controller.Get(new CustomerQueryRequest());

        Assert.IsType<OkObjectResult>(response);
    }

    [Fact]
    public async Task ShouldGetByIdResultOk()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.Exists(It.IsAny<int>()))
           .ReturnsAsync(true);

        service.Setup(x => x.Get(It.IsAny<int>()))
            .ReturnsAsync(new CustomerResult());

        var controller = new CustomersController(service.Object);
        var response = await controller.Get(It.IsAny<int>());

        Assert.IsType<OkObjectResult>(response);
    }

    [Fact]
    public async Task ShouldGetByIdResultNotFound()
    {
        var service = new Mock<ICustomerService>();

        service.Setup(x => x.Exists(It.IsAny<int>()))
           .ReturnsAsync(false);

        var controller = new CustomersController(service.Object);
        var response = await controller.Get(It.IsAny<int>());

        Assert.IsType<NotFoundResult>(response);

        service.Verify(x => x.Get(It.IsAny<int>()), Times.Never);
    }
}