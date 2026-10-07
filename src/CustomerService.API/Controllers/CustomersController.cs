using CustomerService.API.Models.Customers;
using CustomerService.Application.UseCases.Customers.CreateCustomer;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerHandler _handler;

    public CustomersController(CreateCustomerHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(
            request.Name,
            request.Cpf,
            request.Email,
            request.BirthDate);

        var customer = await _handler.Handle(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            customer);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return Ok();
    }
}