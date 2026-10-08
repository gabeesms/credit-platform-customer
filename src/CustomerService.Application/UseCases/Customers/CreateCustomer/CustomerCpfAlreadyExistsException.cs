namespace CustomerService.Application.UseCases.Customers.CreateCustomer;

public sealed class CustomerCpfAlreadyExistsException : Exception
{
    public CustomerCpfAlreadyExistsException(Exception? innerException = null)
        : base("CPF já cadastrado.", innerException)
    {
    }
}