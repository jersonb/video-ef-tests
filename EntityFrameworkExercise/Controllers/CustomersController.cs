using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using EntityFrameworkExercise.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerListResult))]
    public async Task<IActionResult> Get([FromQuery] CustomerQueryRequest query)
    {
        var customers = await customerService.List(query);

        return Ok(customers);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var exists = await customerService.Exists(id);
        if (exists)
        {
            var customer = await customerService.Get(id);
            return Ok(customer);
        }
        return NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CustomerPersistRequest request)
    {
        var exists = await customerService.Exists(id);

        if (exists)
        {
            request.Id = id;
            await customerService.Update(request);
            return NoContent();
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Post(CustomerPersistRequest request)
    {
        int id = await customerService.Create(request);
        return CreatedAtAction(nameof(Get), new { request.Id }, null);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return default;
    }
}