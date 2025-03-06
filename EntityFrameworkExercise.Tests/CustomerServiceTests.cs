using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Services;
using Moq;

namespace EntityFrameworkExercise.Tests;

public class CustomerServiceTests
{
    [Fact]
    public async Task Get() 
    {
        var context = new Mock<IStoreContext>();

        var service = new CustomerService(context.Object);

        var customer = await service.Get(It.IsAny<int>());


    }
}