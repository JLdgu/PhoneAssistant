namespace PhoneAssistant.Tests.Shared;

using System.Globalization;

using PhoneAssistant.WPF.Shared;

using Xunit;

public class EmptyOrNumericValidationRuleTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    void Validate_WithNullOrEmptyString_ReturnsIsValid_True(string? value)
    {
        EmptyOrNumericValidationRule vr = new ();

        var actual = vr.Validate(value, CultureInfo.InvariantCulture);

        Assert.True(actual.IsValid);
    }

    [Fact]
    void Validate_WithNoneNumeric_ReturnsIsValid_False()
    {
        EmptyOrNumericValidationRule vr = new();

        var actual = vr.Validate("Not a number", CultureInfo.InvariantCulture);

        Assert.False(actual.IsValid);
    }

    [Fact]
    void Validate_WithNumeric_ReturnsIsValid_True()
    {
        EmptyOrNumericValidationRule vr = new();

        var actual = vr.Validate("123456", CultureInfo.InvariantCulture);

        Assert.True(actual.IsValid);
    }
}
