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

    [Test]
    public async Task PrintDymoLabelCommand_RequiresLocationOrNonWhitespaceDeliveryAddress()
    {
        TestSetup(_phone);
        _vm.DeliveryAddress = string.Empty;

        await Assert.That(_vm.PrintDymoLabelCommand.CanExecute(null)).IsFalse();

        _vm.DeliveryAddress = "   ";
        await Assert.That(_vm.PrintDymoLabelCommand.CanExecute(null)).IsFalse();

        _vm.DeliveryAddress = "Delivery address";
        await Assert.That(_vm.PrintDymoLabelCommand.CanExecute(null)).IsTrue();

        _vm.DeliveryAddress = string.Empty;
        await Assert.That(_vm.PrintDymoLabelCommand.CanExecute(null)).IsFalse();

        _vm.SelectedLocation = new Location { Name = "Delivery", Address = string.Empty, Collection = false };
        _vm.DeliveryAddress = string.Empty;
        await Assert.That(_vm.PrintDymoLabelCommand.CanExecute(null)).IsTrue();

        _vm.SelectedLocation = null;
        await Assert.That(_vm.PrintDymoLabelCommand.CanExecute(null)).IsFalse();
    }
}
