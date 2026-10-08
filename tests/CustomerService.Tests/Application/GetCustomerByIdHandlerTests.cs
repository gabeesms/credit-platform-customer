using CustomerService.Application.UseCases.Customers.GetCustomerById;
using CustomerService.Domain.Entities;
using CustomerService.Tests.TestDoubles;

namespace CustomerService.Tests.Application;

public class GetCustomerByIdHandlerTests
{
    [Fact]
    public async Task Handle_WhenCustomerExists_ReturnsCustomerAndQueriesRequestedId()
    {
        var repository = new InMemoryCustomerRepository();
        var customer = new Customer("Ana Silva", "52998224725", "ana@example.com", new DateTime(1990, 1, 1));
        repository.CustomerToReturn = customer;
        var handler = new GetCustomerByIdHandler(repository);

        var result = await handler.Handle(new GetCustomerByIdQuery(customer.Id), CancellationToken.None);

        Assert.Same(customer, result);
        Assert.Equal(customer.Id, repository.LastRequestedId);
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var repository = new InMemoryCustomerRepository();
        var handler = new GetCustomerByIdHandler(repository);
        var requestedId = Guid.NewGuid();

        var result = await handler.Handle(new GetCustomerByIdQuery(requestedId), CancellationToken.None);

        Assert.Null(result);
        Assert.Equal(requestedId, repository.LastRequestedId);
    }
}