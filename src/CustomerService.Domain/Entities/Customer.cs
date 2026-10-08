using CustomerService.Domain.ValueObjects;
using System.Net.Mail;

namespace CustomerService.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Cpf CPF { get; private set; } = null!;

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
        var normalizedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ArgumentException("Nome é obrigatório.");

        if (normalizedName.Length > 150)
            throw new ArgumentException("Nome deve ter no máximo 150 caracteres.", nameof(name));

        var normalizedEmail = email?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail))
            throw new ArgumentException("E-mail é obrigatório.", nameof(email));

        if (normalizedEmail.Length > 200)
            throw new ArgumentException("E-mail deve ter no máximo 200 caracteres.", nameof(email));

        if (!MailAddress.TryCreate(normalizedEmail, out var parsedEmail) ||
            !string.Equals(parsedEmail.Address, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("E-mail inválido.", nameof(email));

        if (birthDate > DateTime.UtcNow)
            throw new ArgumentException("Data de nascimento inválida.", nameof(birthDate));

        var now = DateTime.UtcNow;
        Id = Guid.NewGuid();
        Name = normalizedName;
        CPF = Cpf.Create(cpf);
        Email = normalizedEmail;
        BirthDate = birthDate;
        CreatedAt = now;
        UpdatedAt = now;
    }
}