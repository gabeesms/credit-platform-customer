using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Tests.TestDoubles;

internal sealed class InMemoryCustomerRepository : ICustomerRepository
{
    public bool CpfExists { get; set; }

    public Exception? AddException { get; set; }

    public Exception? GetException { get; set; }

    public Customer? CustomerToReturn { get; set; }

    public int AddCalls { get; private set; }

    public int ExistsByCpfCalls { get; private set; }

    public Guid? LastRequestedId { get; private set; }

    public Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        AddCalls++;

        if (AddException is not null)
            throw AddException;

        CustomerToReturn = customer;
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        LastRequestedId = id;

        if (GetException is not null)
            throw GetException;

        return Task.FromResult(CustomerToReturn?.Id == id ? CustomerToReturn : null);
    }

    public Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken)
    {
        ExistsByCpfCalls++;
        return Task.FromResult(CpfExists || CustomerToReturn?.CPF.Value == cpf.Value);
    }
}