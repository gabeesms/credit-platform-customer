namespace CustomerService.Application.UseCases.Customers.CreateCustomer
{
    public record CreateCustomerCommand(
      string Name,
      string Cpf,
      string Email,
      DateTime BirthDate);
}
