using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken);
}