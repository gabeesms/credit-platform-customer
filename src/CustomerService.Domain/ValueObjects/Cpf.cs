namespace CustomerService.Domain.ValueObjects;

public sealed class Cpf
{
    public string Value { get; private set; }

    private Cpf()
    {
        Value = string.Empty;
    }

    private Cpf(string value)
    {
        Value = value;
    }

    public static Cpf Create(string value)
    {
        var digits = new string(value.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            throw new ArgumentException("CPF deve conter 11 dígitos.");

        if (digits.Distinct().Count() == 1)
            throw new ArgumentException("CPF inválido.");

        if (!IsValid(digits))
            throw new ArgumentException("CPF inválido.");

        return new Cpf(digits);
    }

    private static bool IsValid(string cpf)
    {
        var firstDigit = CalculateDigit(cpf[..9]);

        if (firstDigit != cpf[9] - '0')
            return false;

        var secondDigit = CalculateDigit(cpf[..10]);

        return secondDigit == cpf[10] - '0';
    }

    private static int CalculateDigit(string digits)
    {
        var sum = 0;
        var weight = digits.Length + 1;

        foreach (var digit in digits)
        {
            sum += (digit - '0') * weight;
            weight--;
        }

        var remainder = sum % 11;

        return remainder < 2 ? 0 : 11 - remainder;
    }
}