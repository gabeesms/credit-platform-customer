using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _context;

    public CustomerRepository(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> AddAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return customer;
    }

    public async Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByCpfAsync(
    Cpf cpf,
    CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AnyAsync(
                customer => customer.CPF == cpf,
                cancellationToken);
    }
}