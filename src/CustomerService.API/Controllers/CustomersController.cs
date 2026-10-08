using CustomerService.API.Models.Customers;
using CustomerService.Application.UseCases.Customers.CreateCustomer;
using CustomerService.Application.UseCases.Customers.GetCustomerById;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace CustomerService.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerHandler _createCustomerHandler;
    private readonly GetCustomerByIdHandler _getCustomerByIdHandler;

    public CustomersController(
       CreateCustomerHandler createCustomerHandler,
       GetCustomerByIdHandler getCustomerByIdHandler)
    {
        _createCustomerHandler = createCustomerHandler;
        _getCustomerByIdHandler = getCustomerByIdHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    CreateCustomerRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateCustomerCommand(
                request.Name,
                request.Cpf,
                request.Email,
                request.BirthDate);

            var customer = await _createCustomerHandler.Handle(
                command,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                MapToResponse(customer));
        }
        catch (CustomerCpfAlreadyExistsException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "CPF já cadastrado",
                detail: ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dados inválidos",
                detail: ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
      Guid id,
      CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);

        var customer = await _getCustomerByIdHandler.Handle(
            query,
            cancellationToken);

        if (customer is null)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Cliente não encontrado");

        return Ok(MapToResponse(customer));
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.CPF.Value,
            customer.Email,
            customer.BirthDate,
            customer.CreatedAt,
            customer.UpdatedAt);
    }
}