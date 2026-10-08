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
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF é obrigatório.", nameof(value));

        var digitsBuilder = new System.Text.StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (character is >= '0' and <= '9')
            {
                digitsBuilder.Append(character);
                continue;
            }

            if (character is not '.' and not '-')
                throw new ArgumentException("CPF contém caracteres inválidos.", nameof(value));
        }

        var digits = digitsBuilder.ToString();

        if (digits.Length != 11)
            throw new ArgumentException("CPF deve conter 11 dígitos.", nameof(value));

        if (digits.Distinct().Count() == 1)
            throw new ArgumentException("CPF inválido.", nameof(value));

        if (!IsValid(digits))
            throw new ArgumentException("CPF inválido.", nameof(value));

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