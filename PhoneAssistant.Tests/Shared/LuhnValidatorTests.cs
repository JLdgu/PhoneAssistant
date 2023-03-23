using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

[TestClass]
public sealed class LuhnValidatorTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public void IsValid_ReturnsFalse_When_requiredLength_Invalid(int requiredLength)
    {
        const string luhn = "1234";

        var actual = LuhnValidator.IsValid(luhn, requiredLength);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DataRow("361568", 3)]
    [DataRow("361568", 9)]
    public void IsValid_ReturnFalse_When_luhnString_ShorterThan_RequiredLength(string luhn, int requiredLength)
    {       
        var actual = LuhnValidator.IsValid(luhn, requiredLength);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsValid_ReturnsFalse_WhenInputNull()
    {
        string? input = null;

        var actual = LuhnValidator.IsValid(input);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsValid_ReturnsFalse_WhenInputOneDigit()
    {
        string input = "7";

        var actual = LuhnValidator.IsValid(input);

        Assert.IsFalse(actual);

    }
    [TestMethod]
    public void IsValid_ReturnsFalse_WhenInputNotNumeric()
    {
        string input = "6a";

        var actual = LuhnValidator.IsValid(input);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DataRow("79927398713")]
    [DataRow("35145120840121")]
    [DataRow("361568")]
    [DataRow("361576")]
    [DataRow("4012888888881881")]
    public void IsValid_ReturnsTrue_WhenInputValid(string luhnString)
    {        
        // Act
        var actual = LuhnValidator.IsValid(luhnString);

        //Assert
        Assert.IsTrue(actual);
    }
}
