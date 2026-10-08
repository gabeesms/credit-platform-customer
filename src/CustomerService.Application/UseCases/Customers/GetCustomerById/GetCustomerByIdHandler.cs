using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.UseCases.Customers.GetCustomerById;

public class GetCustomerByIdHandler
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer?> Handle(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await _customerRepository.GetByIdAsync(
            query.Id,
            cancellationToken);
    }
}