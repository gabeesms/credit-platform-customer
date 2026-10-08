namespace CustomerService.API.Models.Customers;

public record CustomerResponse(
    Guid Id,
    string Name,
    string Cpf,
    string Email,
    DateTime BirthDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);