using System.Globalization;
using System.Windows.Controls;

using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Shared;

using Xunit;

namespace PhoneAssistant.Tests.Shared;

public sealed class IMEIValidationRuleTests
{
    private readonly IMEIValidationRule _validationRule;
    public IMEIValidationRuleTests()
    {
        _validationRule = new IMEIValidationRule();
    }

    [Fact]
    public void Validate_NullValue_ReturnsValidResult()
    {
        ValidationResult result = _validationRule.Validate(null, CultureInfo.InvariantCulture);
        Assert.Equal(ValidationResult.ValidResult, result);
    }

    [Fact]
    public void Validate_NonStringInput_ReturnsInvalidResult()
    {
        var result = _validationRule.Validate(12345, CultureInfo.InvariantCulture);
        Assert.False(result.IsValid);
        Assert.Equal("IMEI must be empty or 15 digits", result.ErrorContent);
    }

    [Fact]
    public void Validate_EmptyOrWhitespaceIMEI_ReturnsValidResult()
    {
        var result1 = _validationRule.Validate("", CultureInfo.InvariantCulture);
        var result2 = _validationRule.Validate("   ", CultureInfo.InvariantCulture);

        Assert.Equal(ValidationResult.ValidResult, result1);
        Assert.Equal(ValidationResult.ValidResult, result2);
    }

    [Fact]
    public void Validate_ValidIMEI_ReturnsValidResult()
    {
        var validIMEI = "355808981132845"; // A valid 15-digit IMEI
        var result = _validationRule.Validate(validIMEI, CultureInfo.InvariantCulture);

        Assert.Equal(ValidationResult.ValidResult, result);
    }

    [Fact]
    public void Validate_InvalidIMEI_ReturnsInvalidResult()
    {
        var invalidIMEI = "12345678901234"; // An invalid 14-digit IMEI
        var result = _validationRule.Validate(invalidIMEI, CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
        Assert.Equal("IMEI must be empty or 15 digits", result.ErrorContent);
    }
}

public sealed class RequiredIMEIValidationRuleTests
{
    private readonly RequiredIMEIValidationRule _validationRule;
    public RequiredIMEIValidationRuleTests()
    {
        _validationRule = new RequiredIMEIValidationRule();
    }

    [Fact]
    public void Validate_NullValue_ReturnsInvalidResult()
    {
        ValidationResult result = _validationRule.Validate(null, CultureInfo.InvariantCulture);
        Assert.False(result.IsValid);
        Assert.Equal("IMEI must be 15 digits", result.ErrorContent);
    }

    [Fact]
    public void Validate_EmptyOrWhitespaceIMEI_ReturnsInvalidResult()
    {
        var result1 = _validationRule.Validate("", CultureInfo.InvariantCulture);
        var result2 = _validationRule.Validate("   ", CultureInfo.InvariantCulture);

        Assert.False(result1.IsValid);
        Assert.Equal("IMEI must be 15 digits", result1.ErrorContent);
        Assert.False(result2.IsValid);
        Assert.Equal("IMEI must be 15 digits", result2.ErrorContent);
    }

}