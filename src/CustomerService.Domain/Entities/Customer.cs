using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Cpf CPF { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public DateTime BirthDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private Customer()
    {
    }

    public Customer(
        string name,
        string cpf,
        string email,
        DateTime birthDate)
    {
        Id = Guid.NewGuid();
        Name = name;
        CPF = Cpf.Create(cpf);
        Email = email;
        BirthDate = birthDate;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}