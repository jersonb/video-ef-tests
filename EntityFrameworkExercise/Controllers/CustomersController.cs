using EntityFrameworkExercise.Requests;
using EntityFrameworkExercise.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerListResult))]
    public async Task<IActionResult> Get([FromQuery] CustomerQueryRequest query)
    {
        var customers = await service.List(query);
        return Ok(customers);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var exists = await service.Exists(id);
        if (exists)
        {
            var customer = await service.Get(id);
            return Ok(customer);
        }
        return NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CustomerPersistRequest request)
    {
        var exists = await service.Exists(id);

        if (exists)
        {
            request.Id = id;
            await service.Update(request);
            return NoContent();
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Post(CustomerPersistRequest request)
    {
        int id = await service.Create(request);
        return CreatedAtAction(nameof(Get), new { id }, null);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.Delete(id);
        return Ok();
    }
}