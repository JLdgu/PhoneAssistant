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
        v1Phone phone = new()
        {
            PhoneNumber = phoneNumber,
            SimNumber = simNumber,
            Status = status,
            AssetTag = "at",
            FormerUser = "fu",
            Imei = "imei",
            Model = "model",
            NewUser = "nu",
            NorR = "norr",
            Notes = "note",
            OEM = "oem",
            SR = 123456
        };
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();

        Assert.Equal(phone.AssetTag, vm.AssetTag);
        Assert.Equal(phone.FormerUser, vm.FormerUser);
        Assert.Equal(phone.Imei, vm.Imei);
        Assert.Equal(phone.Model, vm.Model);
        Assert.Equal(phone.NewUser, vm.NewUser);
        Assert.Equal(phone.NorR, vm.NorR);
        Assert.Equal(phone.Notes, vm.Notes);
        Assert.Equal(phone.OEM, vm.OEM);
        Assert.Equal(phone.PhoneNumber ?? string.Empty, vm.PhoneNumber);
        Assert.Equal(phone.SimNumber ?? string.Empty, vm.SimNumber);
        Assert.Equal(phone.SR.ToString(), vm.SR);
        Assert.Equal(phone.Status, vm.Status);
        Assert.Equal(canRemoveSim, vm.CanRemoveSim);
        Assert.Equal(canPrintEnvelope, vm.CanPrintEnvelope);
    }

    #region Update
    [Fact]
    private void OnAssetTagChanged_CallsUpdateAsync_WithChangedValue()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");        

        vm.AssetTag = "Updated";

        repository.Verify(r => r.UpdateAsync(phone), Times.Once);
        Assert.Equal("Updated", phone.AssetTag);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    [Theory]
    [InlineData("Former User", "Former User")]
    [InlineData("", null)]
    private void OnFormerUserChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            FormerUser = "formeruser",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");

        vm.FormerUser = actual;

        repository.Verify(r => r.UpdateAsync(phone), Times.Once);
        Assert.Equal(expected, phone.FormerUser);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    [Fact]
    private void OnModelChanged_CallsUpdateAsync_WithChangedValue()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");

        vm.Model = "Updated";

        repository.Verify(r => r.UpdateAsync(phone), Times.Once);
        Assert.Equal("Updated", phone.Model);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    [Theory]
    [InlineData("New User", "New User")]
    [InlineData("", null)]
    private void OnNewUserChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            NewUser = "user",
            OEM = "oem",
            Status = "status"
        };
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");

        vm.NewUser = actual;

        repository.Verify(r => r.UpdateAsync(phone), Times.Once);
        Assert.Equal(expected, phone.NewUser);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    [Theory]
    [InlineData("New note", "New note")]
    [InlineData("", null)]
    private void OnNotesChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            Notes = "note",
            OEM = "oem",
            Status = "status"
        };
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");

        vm.Notes = actual;

        repository.Verify(r => r.UpdateAsync(phone), Times.Once);
        Assert.Equal(expected, phone.Notes);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    [Theory]
    [InlineData("345", 345)]
    [InlineData("", null)]
    private void OnSRChanged_CallsUpdateAsync_WithChangedValue(string actual, int? expected)
    {
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status",
            SR = 123
        };
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");

        vm.SR = actual;

        repository.Verify(r => r.UpdateAsync(phone), Times.Once);
        Assert.Equal(expected, phone.SR);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    #endregion

    #region RemoveSim
    [Fact]
    private void RemoveSim_CallsRepository_RemoveSimFromPhone()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();

        vm.RemoveSimCommand.Execute(null);

        repository.Verify(r => r.RemoveSimFromPhone(phone),Times.Once);        
    }

    [Fact]
    private void RemoveSim_SetsBoundProperties()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>(); 
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.RemoveSimFromPhone(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => phone = p)
            .ReturnsAsync("LastUpdate");

        vm.RemoveSimCommand.Execute(null);

        Assert.Equal(string.Empty, vm.PhoneNumber);
        Assert.Equal(string.Empty, vm.SimNumber);
        Assert.Equal("LastUpdate", vm.LastUpdate);
    }

    [Fact]
    private void RemoveSim_SetsCanRemoveSim_False()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();

        vm.RemoveSimCommand.Execute(null);

        Assert.False(vm.CanRemoveSim);
    }
    #endregion

    [Fact]
    private void PrintEnvelopeCommand_CallsPrintEnvelope_Execute()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();
        Mock<IPrintEnvelope> repository = mocker.GetMock<IPrintEnvelope>();

        vm.PrintEnvelopeCommand.Execute(null);

        repository.Verify(p => p.Execute(phone), Times.Once);
    }

    [Fact]
    private void PrintEnvelope_SetsCanPrintEnvelope_False()
    {
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
        AutoMocker mocker = new AutoMocker();
        mocker.Use(phone);
        PhonesItemViewModel vm = mocker.CreateInstance<PhonesItemViewModel>();

        vm.PrintEnvelopeCommand.Execute(null);

        Assert.False(vm.CanPrintEnvelope);
    }
}
