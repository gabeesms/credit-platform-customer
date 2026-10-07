using CustomerService.Domain.Entities;

namespace CustomerService.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken);
}