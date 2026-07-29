using System.Text.RegularExpressions;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

public static partial class AddressReformatter
{
    public static string Reformat(string address)
    {
        var regex = AddressReformat();
        string reformatted = regex.Replace(address, string.Empty);

        foreach (Match match in regex.Matches(address))
        {
            Console.WriteLine("Found match: {0}", match.Value);
        }
        return reformatted;
    }

    [GeneratedRegex(@"C/o\r\n|First line of address\r\n|Second line of address\r\n|Town/city\r\n|Postcode\r\n|County\r\n(?=(?:Devon|Cornwall)\r\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    //[GeneratedRegex(@"C/o\r\n|First line of address\r\n|Second line of address\r\n|Town/City\r\n|County\r\n|Postcode\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    //[GeneratedRegex(@"(?:C/o|First line of address|Second line of address|County|Town/City|Postcode)\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex AddressReformat();
}

internal class AddressFormatterTests
{   
    [Test]
    internal async Task ReformatDeliveryAddress_should_not_strip_County_in_content()
    {
    string actual = AddressReformatter.Reformat("""
            C/o
            User Name
            First line of address
            Devon County Council
            Second line of address
            County
            Town/city
            Topsham Road
            County
            Devon
            Postcode
            EX31 3UD
            """);

        await Assert.That(actual).IsEqualTo("""
            User Name
            Devon County Council
            County
            Topsham Road
            Devon
            EX31 3UD
            """);
    }

    [Test]
    internal async Task ReformatDeliveryAddress_should_strip_headings()
    {
        string actual = AddressReformatter.Reformat("""
            C/o
            User Name
            First line of address
            Devon County Council
            Second line of address
            Fishleigh Road
            Town/city
            Barnstaple
            County
            Devon
            Postcode
            EX31 3UD
            """);

        await Assert.That(actual).IsEqualTo("""
            User Name
            Devon County Council
            Fishleigh Road
            Barnstaple
            Devon
            EX31 3UD
            """);
    }

}
