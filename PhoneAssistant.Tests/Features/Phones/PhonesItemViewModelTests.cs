using Moq;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesItemViewModelTests
{
    [Theory]
    [InlineData("phone number", "sim number",true)]
    [InlineData(null, "sim number",false)]
    [InlineData("phone number", null,false)]
    [InlineData(null, null, false)]
    private void PhonePropertySet_SetsBoundProperties(string? phoneNumber, string? simNumber, bool canExecute)
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
            Status = "status"
        };

        vm.Phone = phone;

        Assert.Equal(phone.PhoneNumber ?? string.Empty, vm.PhoneNumber);
        Assert.Equal(phone.SimNumber ?? string.Empty, vm.SimNumber);
        Assert.Equal(canExecute, vm.CanRemoveSim);
    }

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
        vm.RemoveSim();

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
        vm.RemoveSim();

        Assert.Equal(string.Empty, vm.PhoneNumber);
        Assert.Equal(string.Empty, vm.SimNumber);
    }

    [Fact]
    private void RemoveSim_SetsCanExecute_False()
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
        vm.RemoveSim();

        Assert.False(vm.CanRemoveSim);
    }
}
