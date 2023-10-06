using Moq;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesItemViewModelTests
{
    [Theory]
    [InlineData("phone number", "sim number",true,"In Stock",false)]
    [InlineData(null, "sim number",false,"In Stock",false)]
    [InlineData("phone number", null,false,"In Stock", false)]
    [InlineData(null, null, false, "In Stock", false)]
    [InlineData("phone number", "sim number", true, "Production", true)]
    [InlineData(null, "sim number", false, "Production", true)]
    [InlineData("phone number", null, false, "Production", true)]
    [InlineData(null, null, false, "Production", true)]
    private void PhonePropertySet_SetsBoundProperties(string? phoneNumber, string? simNumber, bool canRemoveSim, string status, bool canPrintEnvelope)
    {
        AutoMocker mocker = new AutoMocker();
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = phoneNumber,
            SimNumber = simNumber,
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = status
        };

        vm.Phone = phone;

        Assert.Equal(phone.PhoneNumber ?? string.Empty, vm.PhoneNumber);
        Assert.Equal(phone.SimNumber ?? string.Empty, vm.SimNumber);
        Assert.Equal(canRemoveSim, vm.CanRemoveSim);
        Assert.Equal(canPrintEnvelope, vm.CanPrintEnvelope);
    }
    #region RemoveSim
    [Fact]
    private void RemoveSim_CallsRepository_RemoveSimFromPhone()
    {
        AutoMocker mocker = new AutoMocker();
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };

        vm.Phone = phone;
        vm.RemoveSimCommand.Execute(null);

        repository.Verify(r => r.RemoveSimFromPhone(phone),Times.Once);        
    }

    [Fact]
    private void RemoveSim_ClearsBoundProperties()
    {
        AutoMocker mocker = new AutoMocker();
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };

        vm.Phone = phone;
        vm.RemoveSimCommand.Execute(null);

        Assert.Equal(string.Empty, vm.PhoneNumber);
        Assert.Equal(string.Empty, vm.SimNumber);
    }

    [Fact]
    private void RemoveSim_SetsCanRemoveSim_False()
    {
        AutoMocker mocker = new AutoMocker();
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };

        vm.Phone = phone;
        vm.RemoveSimCommand.Execute(null);

        Assert.False(vm.CanRemoveSim);
    }
    #endregion

    [Fact]
    private void PrintEnvelopeCommand_CallsPrintEnvelope_Execute()
    {
        AutoMocker mocker = new AutoMocker();
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPrintEnvelope> repository = mocker.GetMock<IPrintEnvelope>();
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };

        vm.Phone = phone;
        vm.PrintEnvelopeCommand.Execute(null);

        repository.Verify(p => p.Execute(phone), Times.Once);
    }

    [Fact]
    private void PrintEnvelope_SetsCanPrintEnvelope_False()
    {
        AutoMocker mocker = new AutoMocker();
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "Production"
        };

        vm.Phone = phone;
        vm.PrintEnvelopeCommand.Execute(null);

        Assert.False(vm.CanPrintEnvelope);
    }
}
