using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

[TestClass]
public sealed class LuhnValidatorTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public void IsValid_ThrowsException_When_requiredLength_Invalid(int requiredLength)
    {
        const string input = "aaa";

        var actual = Assert.ThrowsException<ArgumentException>(() => LuhnValidator.IsValid(input, requiredLength));

        Assert.AreEqual("length must be omitted or > 1", actual.Message);
    }

    [TestMethod]
    public void IsValid_ThrowsException_When_luhnString_ShorterThan_RequiredLength()
    {
        const string input = "361568";

        var actual = Assert.ThrowsException<ArgumentException>(() => LuhnValidator.IsValid(input, 99));

        Assert.AreEqual("Length of luhnString != length", actual.Message);
    }

    [TestMethod]
    public void IsValid_ThrowsException_When_luhnString_LongerThan_RequiredLength()
    {
        const string input = "361568";

        var actual = Assert.ThrowsException<ArgumentException>(() => LuhnValidator.IsValid(input, 3));

        Assert.AreEqual("Length of luhnString != length", actual.Message);
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
