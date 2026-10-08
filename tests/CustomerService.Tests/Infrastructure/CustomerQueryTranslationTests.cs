using CustomerService.Domain.ValueObjects;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Tests.Infrastructure;

public class CustomerQueryTranslationTests
{
    [Fact]
    public void CpfEqualityFilter_IsTranslatedBySqlServerProvider()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseSqlServer("Server=localhost;Database=CustomerServiceDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        using var context = new CustomerDbContext(options);
        var cpf = Cpf.Create("52998224725");

        var sql = context.Customers
            .Where(customer => customer.CPF == cpf)
            .ToQueryString();

        Assert.Contains("WHERE [c].[CPF] = @cpf", sql);
    }
}