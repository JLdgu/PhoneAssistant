using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

public sealed class LuhnValidatorTests
{
    [Test]
    [Arguments(0)]
    [Arguments(1)]
    public async Task IsValid_ReturnsFalse_When_requiredLength_InvalidAsync(int requiredLength)
    {
        const string luhn = "1234";

        var actual = LuhnValidator.IsValid(luhn, requiredLength);

        await Assert.That(actual).IsFalse();
    }

    [Test]
    [Arguments("361568", 3)]
    [Arguments("361568", 9)]
    public async Task IsValid_ReturnFalse_When_luhnString_ShorterThan_RequiredLengthAsync(string luhn, int requiredLength)
    {       
        var actual = LuhnValidator.IsValid(luhn, requiredLength);

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task IsValid_ReturnsFalse_WhenInputNullAsync()
    {
        string? input = null;

        var actual = LuhnValidator.IsValid(input);

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task IsValid_ReturnsFalse_WhenInputOneDigitAsync()
    {
        string input = "7";

        var actual = LuhnValidator.IsValid(input);

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task IsValid_ReturnsFalse_WhenInputNotNumericAsync()
    {
        string input = "6a";

        var actual = LuhnValidator.IsValid(input);

        await Assert.That(actual).IsFalse();
    }

    [Test]
    [Arguments("79927398713")]
    [Arguments("35145120840121")]
    [Arguments("361568")]
    [Arguments("361576")]
    [Arguments("4012888888881881")]
    public async Task IsValid_ReturnsTrue_WhenInputValidAsync(string luhnString)
    {        
        // Act
        var actual = LuhnValidator.IsValid(luhnString);

        //Assert
        await Assert.That(actual).IsTrue();
    }
}
