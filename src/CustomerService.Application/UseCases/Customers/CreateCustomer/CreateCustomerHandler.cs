using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.UseCases.Customers.CreateCustomer;

public class CreateCustomerHandler
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var customer = new Customer(
            command.Name,
            command.Cpf,
            command.Email,
            command.BirthDate);

        return await _customerRepository.AddAsync(
            customer,
            cancellationToken);
    }
}