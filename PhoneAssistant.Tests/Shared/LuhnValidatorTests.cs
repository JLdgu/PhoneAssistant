using PhoneAssistant.WPF.Shared;

using Xunit;

namespace PhoneAssistant.Tests.Shared;

public sealed class LuhnValidatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void IsValid_ReturnsFalse_When_requiredLength_Invalid(int requiredLength)
    {
        const string luhn = "1234";

        var actual = LuhnValidator.IsValid(luhn, requiredLength);

        Xunit.Assert.False(actual);
    }

    [Theory]
    [InlineData("361568", 3)]
    [InlineData("361568", 9)]
    public void IsValid_ReturnFalse_When_luhnString_ShorterThan_RequiredLength(string luhn, int requiredLength)
    {       
        var actual = LuhnValidator.IsValid(luhn, requiredLength);

        Xunit.Assert.False(actual);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenInputNull()
    {
        string? input = null;

        var actual = LuhnValidator.IsValid(input);

        Xunit.Assert.False(actual);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenInputOneDigit()
    {
        string input = "7";

        var actual = LuhnValidator.IsValid(input);

        Xunit.Assert.False(actual);

    }
    [Fact]
    public void IsValid_ReturnsFalse_WhenInputNotNumeric()
    {
        string input = "6a";

        var actual = LuhnValidator.IsValid(input);

        Xunit.Assert.False(actual);
    }

    [Theory]
    [InlineData("79927398713")]
    [InlineData("35145120840121")]
    [InlineData("361568")]
    [InlineData("361576")]
    [InlineData("4012888888881881")]
    public void IsValid_ReturnsTrue_WhenInputValid(string luhnString)
    {        
        // Act
        var actual = LuhnValidator.IsValid(luhnString);

        //Assert
        Xunit.Assert.True(actual);
    }
}
