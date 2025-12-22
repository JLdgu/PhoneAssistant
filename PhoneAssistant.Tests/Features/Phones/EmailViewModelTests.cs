using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Shared;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class EmailViewModelTests
{
    private readonly Phone _phone = new()
    {
        PhoneNumber = "phoneNumber",
        SimNumber = "simNumber",
        Status = "status",
        AssetTag = "at",
        DespatchDetails = "dd",
        FormerUser = "fu",
        Imei = "imei",
        Model = "model",
        NewUser = "nu",
        Condition = "norr",
        Notes = "note",
        OEM = Manufacturer.Apple,
        Ticket = 123456
    };

    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly Mock<IPhonesRepository> _phones;

    private readonly EmailViewModel _vm;

    public EmailViewModelTests()
    {
        _phones = _mocker.GetMock<IPhonesRepository>();

        _vm = _mocker.CreateInstance<EmailViewModel>();
    }

    private void TestSetup(Phone phone)
    {
        OrderDetails orderDetails = new(phone);
        _vm.OrderDetails = orderDetails;
    }

    [Test]
    public async Task CloseCommand_SetsGeneratingEmail_False()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        await Assert.That(_vm.GeneratingEmail).IsFalse();
    }

    [Test]
    public void CloseCommand_UpdatesDb()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        _phones.Verify(r => r.UpdateAsync(_phone), Times.Once);
    }

    [Test]
    public async Task Constructor_SetsGeneratingEmail_True()
    {
        TestSetup(_phone);

        await Assert.That(_vm.GeneratingEmail).IsTrue();
    }
        
    [Test]
    public async Task OrderDetails_ShouldSetDeviceTypePhone_WhenModelDoesNotContanIPad()
    {
        TestSetup(_phone);

        await Assert.That(_vm.OrderDetails.DeviceType).IsEqualTo(DeviceType.Phone);
    }

    [Test]
    public async Task OrderDetails_ShouldSetDeviceTypeTablet_WhenModelContainsIPad()
    {
        _phone.Model = "iPad";
        TestSetup(_phone);

        await Assert.That(_vm.OrderDetails.DeviceType).IsEqualTo(DeviceType.Tablet);
    }

    [Test]
    public async Task OrderDetails_ShouldSetOrderTypeNew_WhenPhoneDetailsSupplied()
    {
        TestSetup(_phone);

        await Assert.That(_vm.OrderType).IsEqualTo(OrderType.New);
    }

    [Test]
    public async Task OrderDetails_ShouldSetOrderTypeReplacement_WhenPhoneDetailsNotSupplied()
    {
        _phone.PhoneNumber = null;
        _phone.SimNumber = null;
        TestSetup(_phone);

        await Assert.That(_vm.OrderType).IsEqualTo(OrderType.Replacement);
    }

    [Test]
    public async Task ReformatDeliveryAddress_ShouldStripHeadingsAsync()
    {
        string actual = EmailViewModel.ReformatDeliveryAddress("""
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

    [Test]
    public async Task SelectedLocation_InterpolatesValuesFor_DeliveryAddress()
    {
        _phone.NewUser = "New User";
        _phone.Ticket = 42;
        _phone.PhoneNumber = "999";
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect", Address = "{NewUser}, {SR}, {PhoneNumber}", Collection = true };

        await Assert.That(_vm.DeliveryAddress).Contains(_phone.NewUser);
        await Assert.That(_vm.DeliveryAddress).Contains(_phone.Ticket.ToString()!);
        await Assert.That(_vm.DeliveryAddress).Contains(_phone.PhoneNumber);
    }
}
