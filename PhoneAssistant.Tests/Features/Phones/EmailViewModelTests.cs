using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class EmailViewModelTests
{
    private readonly v1Phone _phone = new()
    {
        PhoneNumber = "phoneNumber",
        SimNumber = "simNumber",
        Status = "status",
        AssetTag = "at",
        FormerUser = "fu",
        Imei = "imei",
        LastUpdate = "lastupdate",
        Model = "model",
        NewUser = "nu",
        NorR = "norr",
        Notes = "note",
        OEM = "oem",
        SR = 123456
    };

    [Fact]
    private void EmailHtml_WithOrderTypeNew()
    {
        EmailViewModel vm = new EmailViewModel();
        vm.SetupEmail(_phone);

        vm.OrderType = OrderType.New;

        Assert.Contains("<td>Order type:</td><td>New</td>", vm.EmailHtml);
    }
    [Fact]
    private void EmailHtml_WithOrderTypeReplacement()
    {
        EmailViewModel vm = new EmailViewModel();
        vm.SetupEmail(_phone);

        vm.OrderType = OrderType.Replacement;

        Assert.Contains("<td>Order type:</td><td>Replacement</td>", vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithDespatchMethodColletGMH()
    {
        EmailViewModel vm = new EmailViewModel();
        vm.SetupEmail(_phone);

        vm.DespatchMethod = DespatchMethod.CollectGMH;

        Assert.Contains("Your phone can be collected from", vm.EmailHtml);
        Assert.Contains("Hardware Room, Great Moor House", vm.EmailHtml);
    }
    [Fact]
    private void EmailHtml_WithDespatchMethodColletL87()
    {
        EmailViewModel vm = new EmailViewModel();
        vm.SetupEmail(_phone);

        vm.DespatchMethod = DespatchMethod.CollectL87;

        Assert.Contains("Your phone can be collected from", vm.EmailHtml);
        Assert.Contains("Room L87, County Hall", vm.EmailHtml);
    }

    [Theory]
    [InlineData("N", "New")]
    [InlineData("R", "Repurposed")]
   
    private void EmailHtml_ContainsPhoneDetails(string norr, string norrDescription)
    {
        EmailViewModel vm = new EmailViewModel();
        _phone.NorR = norr;
        vm.SetupEmail(_phone);

        Assert.Contains($"<td>Phone supplied:</td><td>{norrDescription} {_phone.OEM} {_phone.Model}</td>", vm.EmailHtml);
    }
}
