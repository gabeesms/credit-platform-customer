using CustomerService.Domain.ValueObjects;

namespace CustomerService.Tests.Domain;

public class CpfTests
{
    [Fact]
    public void Create_WithValidCpf_ReturnsCpf()
    {
        var cpf = Cpf.Create("52998224725");

        Assert.Equal("52998224725", cpf.Value);
    }

    [Fact]
    public void Create_WithInvalidCheckDigits_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Cpf.Create("52998224724"));
    }

    [Fact]
    public void Create_WithWrongNumberOfDigits_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Cpf.Create("1234567890"));
    }

    [Fact]
    public void Create_WithRepeatedDigits_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Cpf.Create("11111111111"));
    }

    [Fact]
    public void Create_WithStandardFormatting_NormalizesCpf()
    {
        var cpf = Cpf.Create("529.982.247-25");

        Assert.Equal("52998224725", cpf.Value);
    }

    [Fact]
    public void Create_WithLetters_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Cpf.Create("abc52998224725"));
    }

    [Fact]
    public void Create_WithNonAsciiDigits_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Cpf.Create("٥٢٩٩٨٢٢٤٧٢٥"));
    }

    [Fact]
    public void Create_WithNullOrWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Cpf.Create(" "));
    }
}