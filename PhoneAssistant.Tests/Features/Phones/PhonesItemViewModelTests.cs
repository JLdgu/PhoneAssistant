using Moq;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesItemViewModelTests
{
    private v1Phone _phone = new()
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
    private readonly v1Phone _updatedPhone = new()
    {
        PhoneNumber = null,
        SimNumber = null,
        Status = "status",
        AssetTag = "at",
        FormerUser = "fu",
        Imei = "imei",
        LastUpdate = "updated",
        Model = "model",
        NewUser = "nu",
        NorR = "norr",
        Notes = "note",
        OEM = "oem",
        SR = 123456
    };

    private readonly AutoMocker _mocker = new AutoMocker();
    private PhonesItemViewModel _vm;
    private readonly Mock<IPhonesRepository> _repository;

    public PhonesItemViewModelTests()
    {
        _mocker.Use(_phone);
        _vm = _mocker.CreateInstance<PhonesItemViewModel>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.UpdateAsync(It.IsAny<v1Phone>()))
            .Callback<v1Phone>((p) => _phone = p)
            .ReturnsAsync("LastUpdate");
        _repository.Setup(r => r.RemoveSimFromPhone(It.IsAny<v1Phone>()))            
            .ReturnsAsync(_updatedPhone);
    }

    [Theory]
    [InlineData("phone number", "sim number","In Stock",false)]
    [InlineData(null, "sim number","In Stock",false)]
    [InlineData("phone number", null,"In Stock", false)]
    [InlineData(null, null, "In Stock", false)]
    [InlineData("phone number", "sim number",  "Production", true)]
    [InlineData(null, "sim number", "Production", true)]
    [InlineData("phone number", null, "Production", true)]
    [InlineData(null, null, "Production", true)]
    private void PhonePropertySet_SetsBoundProperties(string? phoneNumber, string? simNumber, string status, bool canPrintEnvelope)
    {
        _phone.PhoneNumber = phoneNumber;
        _phone.SimNumber = simNumber;
        _phone.Status = status;
        _vm = _mocker.CreateInstance<PhonesItemViewModel>();

        Assert.Equal(_phone.AssetTag, _vm.AssetTag);
        Assert.Equal(_phone.FormerUser, _vm.FormerUser);
        Assert.Equal(_phone.Imei, _vm.Imei);
        Assert.Equal(_phone.Model, _vm.Model);
        Assert.Equal(_phone.NewUser, _vm.NewUser);
        Assert.Equal(_phone.NorR, _vm.NorR);
        Assert.Equal(_phone.Notes, _vm.Notes);
        Assert.Equal(_phone.OEM, _vm.OEM);
        Assert.Equal(_phone.PhoneNumber ?? string.Empty, _vm.PhoneNumber);
        Assert.Equal(_phone.SimNumber ?? string.Empty, _vm.SimNumber);
        Assert.Equal(_phone.SR.ToString(), _vm.SR);
        Assert.Equal(_phone.Status, _vm.Status);
        Assert.Equal(canPrintEnvelope, _vm.CanPrintEnvelope);
    }

    #region Update
    [Fact]
    private void OnAssetTagChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.AssetTag = "Updated";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("Updated", _phone.AssetTag);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Theory]
    [InlineData("Former User", "Former User")]
    [InlineData("", null)]
    private void OnFormerUserChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.FormerUser = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.FormerUser);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Fact]
    private void OnModelChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.Model = "Updated";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("Updated", _phone.Model);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Theory]
    [InlineData("New User", "New User")]
    [InlineData("", null)]
    private void OnNewUserChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.NewUser = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.NewUser);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Fact]
    private void OnNorRChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.NorR = "changed";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("changed", _phone.NorR);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Theory]
    [InlineData("New note", "New note")]
    [InlineData("", null)]
    private void OnNotesChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.Notes = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.Notes);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Fact]
    private void OnOEMChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.OEM = "new OEM";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("new OEM", _phone.OEM);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }
    
    [Theory]
    [InlineData("345", 345)]
    [InlineData("", null)]
    private void OnSRChanged_CallsUpdateAsync_WithChangedValue(string actual, int? expected)
    {
        _vm.SR = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.SR);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    [Fact]
    private void OnStatusChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.Status = "changed";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("changed", _phone.Status);
        Assert.Equal("LastUpdate", _vm.LastUpdate);
    }

    #endregion

    #region RemoveSim
    [Fact]
    private void RemoveSim_CallsRepository_RemoveSimFromPhone()
    {
        _vm.RemoveSimCommand.Execute(null);

        _repository.Verify(r => r.RemoveSimFromPhone(_phone),Times.Once);        
    }

    [Fact]
    private void RemoveSim_SetsBoundProperties()
    {
        _vm.RemoveSimCommand.Execute(null);

        Assert.Equal(string.Empty, _vm.PhoneNumber);
        Assert.Equal(string.Empty, _vm.SimNumber);
        Assert.Equal(_updatedPhone.LastUpdate, _vm.LastUpdate);
    }

    [Fact]
    private void RemoveSim_SetsCanRemoveSim_False()
    {

        _vm.RemoveSimCommand.Execute(null);

        Assert.False(_vm.CanRemoveSim());
    }
    #endregion

    [Fact]
    private void PrintEnvelopeCommand_CallsPrintEnvelope_Execute()
    {
        Mock<IPrintEnvelope> repository = _mocker.GetMock<IPrintEnvelope>();

        _vm.PrintEnvelopeCommand.Execute(null);

        repository.Verify(p => p.Execute(_phone), Times.Once);
    }

    [Fact]
    private void PrintEnvelope_SetsCanPrintEnvelope_False()
    {
        _vm.PrintEnvelopeCommand.Execute(null);

        Assert.False(_vm.CanPrintEnvelope);
    }
}
