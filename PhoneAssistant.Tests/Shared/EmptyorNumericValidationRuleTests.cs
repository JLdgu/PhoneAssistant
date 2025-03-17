namespace PhoneAssistant.Tests.Shared;

using System.Globalization;
using System.Threading.Tasks;

using PhoneAssistant.WPF.Shared;

public class EmptyOrNumericValidationRuleTests
{
    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task Validate_WithNullOrEmptyString_ReturnsIsValid_TrueAsync(string? value)
    {
        EmptyOrNumericValidationRule vr = new ();

#pragma warning disable CS8604 // Possible null reference argument.
        var actual = vr.Validate(value, CultureInfo.InvariantCulture);
#pragma warning restore CS8604 // Possible null reference argument.

        await Assert.That(actual.IsValid).IsTrue();
    }

    [Test]
    public async Task Validate_WithNoneNumeric_ReturnsIsValid_False()
    {
        EmptyOrNumericValidationRule vr = new();

        var actual = vr.Validate("Not a number", CultureInfo.InvariantCulture);

        await Assert.That(actual.IsValid).IsFalse();
    }

    [Test]
    public async Task Validate_WithNumeric_ReturnsIsValid_True()
    {
        EmptyOrNumericValidationRule vr = new();

        var actual = vr.Validate("123456", CultureInfo.InvariantCulture);

        await Assert.That(actual.IsValid).IsTrue();
    }
}
