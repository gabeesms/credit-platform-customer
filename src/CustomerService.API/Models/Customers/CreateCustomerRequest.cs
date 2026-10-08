using System.ComponentModel.DataAnnotations;

namespace CustomerService.API.Models.Customers;

public record CreateCustomerRequest(
    [param: Required]
    [param: StringLength(150, MinimumLength = 1)]
    string Name,
    [param: Required]
    string Cpf,
    [param: Required]
    [param: StringLength(200, MinimumLength = 1)]
    [param: EmailAddress]
    string Email,
    [param: Required]
    DateTime BirthDate);