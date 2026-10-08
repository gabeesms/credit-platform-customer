using CustomerService.Application.UseCases.Customers.CreateCustomer;
using CustomerService.Tests.TestDoubles;

namespace CustomerService.Tests.Application;

public class CreateCustomerHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCustomer_CreatesCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var handler = new CreateCustomerHandler(repository);

        var customer = await handler.Handle(
            new CreateCustomerCommand("Ana Silva", "529.982.247-25", "ana@example.com", new DateTime(1990, 1, 1)),
            CancellationToken.None);

        Assert.Equal("Ana Silva", customer.Name);
        Assert.Equal("52998224725", customer.CPF.Value);
        Assert.Equal("ana@example.com", customer.Email);
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public async Task Handle_WithDuplicateCpf_ThrowsAndDoesNotAddCustomer()
    {
        var repository = new InMemoryCustomerRepository { CpfExists = true };
        var handler = new CreateCustomerHandler(repository);

        await Assert.ThrowsAsync<CustomerCpfAlreadyExistsException>(() => handler.Handle(
            new CreateCustomerCommand("Ana Silva", "52998224725", "ana@example.com", new DateTime(1990, 1, 1)),
            CancellationToken.None));

        Assert.Equal(1, repository.ExistsByCpfCalls);
        Assert.Equal(0, repository.AddCalls);
    }

    [Theory]
    [InlineData("", "ana@example.com")]
    [InlineData("Ana Silva", "email-invalido")]
    public async Task Handle_WithInvalidDomainData_DoesNotAddCustomer(string name, string email)
    {
        var repository = new InMemoryCustomerRepository();
        var handler = new CreateCustomerHandler(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(
            new CreateCustomerCommand(name, "52998224725", email, new DateTime(1990, 1, 1)),
            CancellationToken.None));

        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public async Task Handle_WithNameLongerThanDatabaseLimit_DoesNotAddCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var handler = new CreateCustomerHandler(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(
            new CreateCustomerCommand(new string('A', 151), "52998224725", "ana@example.com", new DateTime(1990, 1, 1)),
            CancellationToken.None));

        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public async Task Handle_WithEmailLongerThanDatabaseLimit_DoesNotAddCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var handler = new CreateCustomerHandler(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(
            new CreateCustomerCommand("Ana Silva", "52998224725", $"{new string('a', 190)}@example.com", new DateTime(1990, 1, 1)),
            CancellationToken.None));

        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public async Task Handle_WithFutureBirthDate_DoesNotAddCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var handler = new CreateCustomerHandler(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(
            new CreateCustomerCommand("Ana Silva", "52998224725", "ana@example.com", DateTime.UtcNow.AddDays(1)),
            CancellationToken.None));

        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReportsDuplicateCpf_PropagatesConflict()
    {
        var repository = new InMemoryCustomerRepository
        {
            AddException = new CustomerCpfAlreadyExistsException()
        };
        var handler = new CreateCustomerHandler(repository);

        await Assert.ThrowsAsync<CustomerCpfAlreadyExistsException>(() => handler.Handle(
            new CreateCustomerCommand("Ana Silva", "52998224725", "ana@example.com", new DateTime(1990, 1, 1)),
            CancellationToken.None));
    }
}