namespace CustomerService.API.Models.Customers;

public record CreateCustomerRequest(
    string Name,
    string Cpf,
    string Email,
    DateTime BirthDate);