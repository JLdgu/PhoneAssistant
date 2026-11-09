using System.Text.RegularExpressions;

namespace PhoneAssistant.WPF.Shared;

public static partial class AddressFormatter
{
    public static string ReformatDeliveryAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return string.Empty;

        var regex = AddressReformat();
        string reformatted = regex.Replace(address, string.Empty);
        return reformatted;
    }

    //[GeneratedRegex(@"C/o\r\n|First line of address\r\n|Second line of address\r\n|Town/City\r\n|County\r\n|Postcode\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    [GeneratedRegex(@"C/o\r\n|First line of address\r\n|Second line of address\r\n|Town/city\r\n|Postcode\r\n|County\r\n(?=(?:Devon|Cornwall)\r\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex AddressReformat();
}
