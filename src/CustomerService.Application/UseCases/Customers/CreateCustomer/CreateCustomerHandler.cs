using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;

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
        var cpf = Cpf.Create(command.Cpf);

        var cpfExists = await _customerRepository.ExistsByCpfAsync(
            cpf,
            cancellationToken);

        if (cpfExists)
            throw new InvalidOperationException("CPF já cadastrado.");

        var customer = new Customer(
            command.Name,
            cpf.Value,
            command.Email,
            command.BirthDate);

        return await _customerRepository.AddAsync(
            customer,
            cancellationToken);
    }
}