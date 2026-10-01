using PhoneAssistant.WPF.Shared;
using System.Globalization;

namespace PhoneAssistant.Tests.Shared;

internal sealed class UtilityTests
{
    [Test]
    [Arguments("13/03/2025", 2, "Monday 17<sup>th</sup> March 2025")]
    [Arguments("14/03/2025", 2, "Tuesday 18<sup>th</sup> March 2025")]
    internal async Task ToOrdinalWorkingDate_AddsCollectionBuffer(string date, int buffer, string expected)
    {
        CultureInfo culture = new("en-GB");
        string actual = Utility.ToOrdinalWorkingDate(DateTime.Parse(date, culture), buffer: buffer);

        await Assert.That(actual).IsEqualTo(expected);
    }

    [Test]
    [Arguments("1/1/2024", "Monday 1<sup>st</sup> January 2024")]
    [Arguments("2/1/2024", "Tuesday 2<sup>nd</sup> January 2024")]
    [Arguments("3/1/2024", "Wednesday 3<sup>rd</sup> January 2024")]
    [Arguments("21/12/2023", "Thursday 21<sup>st</sup> December 2023")]
    [Arguments("22/12/2023", "Friday 22<sup>nd</sup> December 2023")]
    [Arguments("23/1/2024", "Tuesday 23<sup>rd</sup> January 2024")]
    [Arguments("31/1/2024", "Wednesday 31<sup>st</sup> January 2024")]
    [Arguments("12/02/2024", "Monday 12<sup>th</sup> February 2024")] // Issue #40
    internal async Task ToOrdinalWorkingDate_FormatsOutput(string date, string expected)
    {
        CultureInfo culture = new("en-GB");
        string actual = Utility.ToOrdinalWorkingDate(DateTime.Parse(date, culture));

        await Assert.That(actual).IsEqualTo(expected);
    }

    [Test]
    [Arguments("1/1/2024", "Monday 1\x02E2\x1D57 January 2024")]
    [Arguments("2/1/2024", "Tuesday 2\x207F\x1D48 January 2024")]
    [Arguments("3/1/2024", "Wednesday 3\x02B3\x1D48 January 2024")]
    [Arguments("21/12/2023", "Thursday 21\x02E2\x1D57 December 2023")]
    [Arguments("22/12/2023", "Friday 22\x207F\x1D48 December 2023")]
    [Arguments("23/1/2024", "Tuesday 23\x02B3\x1D48 January 2024")]
    [Arguments("31/1/2024", "Wednesday 31\x02E2\x1d57 January 2024")]
    [Arguments("12/02/2024", "Monday 12\x1D57\x02B0 February 2024")] // Issue #40
    internal async Task ToOrdinalWorkingDate_WithHexSuperscriptAsync(string date, string expected)
    {
        CultureInfo culture = new("en-GB");
        string actual = Utility.ToOrdinalWorkingDate(DateTime.Parse(date, culture), true);

        await Assert.That(actual).IsEqualTo(expected);
    }
}
