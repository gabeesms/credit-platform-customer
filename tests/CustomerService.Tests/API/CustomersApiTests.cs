using System.Net;
using System.Net.Http.Json;
using CustomerService.API.Models.Customers;
using CustomerService.Application.UseCases.Customers.CreateCustomer;
using CustomerService.Domain.Entities;
using CustomerService.Tests.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CustomerService.Tests.API;

public class CustomersApiTests
{
    [Fact]
    public async Task PostCustomer_WithValidRequest_ReturnsCreated()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/customers", ValidRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task PostCustomer_WithInvalidCpf_ReturnsBadRequestProblemDetails()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/customers", ValidRequest() with { Cpf = "11111111111" });
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, problem?.Status);
    }

    [Fact]
    public async Task PostCustomer_WithDuplicateCpf_ReturnsConflict()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();
        factory.Repository.CpfExists = true;

        var response = await client.PostAsJsonAsync("/api/customers", ValidRequest());
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(StatusCodes.Status409Conflict, problem?.Status);
    }

    [Fact]
    public async Task PostCustomer_WhenDatabaseReportsDuplicateCpf_ReturnsConflict()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();
        factory.Repository.AddException = new CustomerCpfAlreadyExistsException();

        var response = await client.PostAsJsonAsync("/api/customers", ValidRequest());

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task PostCustomer_WithInvalidEmail_ReturnsBadRequest()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/customers", ValidRequest() with { Email = "invalid-email" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_WhenFound_ReturnsOk()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();
        var customer = new Customer("Ana Silva", "52998224725", "ana@example.com", new DateTime(1990, 1, 1));
        factory.Repository.CustomerToReturn = customer;

        var response = await client.GetAsync($"/api/customers/{customer.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_WhenMissing_ReturnsNotFoundProblemDetails()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/customers/{Guid.NewGuid()}");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(StatusCodes.Status404NotFound, problem?.Status);
    }

    [Fact]
    public async Task GetCustomer_WhenUnexpectedErrorOccurs_ReturnsSanitizedServerError()
    {
        using var factory = new CustomersApiFactory();
        using var client = factory.CreateClient();
        factory.Repository.GetException = new InvalidOperationException("database secret detail");

        var response = await client.GetAsync($"/api/customers/{Guid.NewGuid()}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.DoesNotContain("database secret detail", body);
    }

    private static CreateCustomerRequest ValidRequest() => new(
        "Ana Silva",
        "529.982.247-25",
        "ana@example.com",
        new DateTime(1990, 1, 1));
}

internal sealed class CustomersApiFactory : WebApplicationFactory<Program>
{
    public InMemoryCustomerRepository Repository { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<CustomerService.Application.Interfaces.ICustomerRepository>();
            services.AddSingleton<CustomerService.Application.Interfaces.ICustomerRepository>(Repository);
        });
    }
}