using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesItemViewModelTests
{
    private Phone _phone = new()
    {
        PhoneNumber = "phoneNumber",
        SimNumber = "simNumber",
        Status = "status",
        AssetTag = "at",
        DespatchDetails = "despatch",
        FormerUser = "fu",
        Imei = "imei",  
        Model = "model",
        NewUser = "nu",
        Condition = "norr",
        Notes = "note",
        OEM = Manufacturer.Apple,
        SR = 123456,
        SerialNumber = "sn",
    };
    private readonly Phone _updatedPhone = new()
    {
        PhoneNumber = null,
        SimNumber = null,
        Status = "status",
        AssetTag = "at",
        FormerUser = "fu",
        Imei = "imei",
        Model = "model",
        NewUser = "nu",
        Condition = "norr",
        Notes = "note",
        OEM = Manufacturer.Apple,
        SR = 123456
    };

    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly PhonesItemViewModel _vm;
    private readonly Mock<IPhonesRepository> _repository;

    public PhonesItemViewModelTests()
    {
        _mocker.Use(_phone);
        _vm = _mocker.CreateInstance<PhonesItemViewModel>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.UpdateAsync(It.IsAny<Phone>()))
            .Callback<Phone>((p) => _phone = p);
    }

    [Test]
    [Arguments("phone number", "sim number","In Stock")]
    [Arguments(null, "sim number","In Stock")]
    [Arguments("phone number", null,"In Stock")]
    [Arguments(null, null, "In Stock")]
    [Arguments("phone number", "sim number",  "Production")]
    [Arguments(null, "sim number", "Production")]
    [Arguments("phone number", null, "Production")]
    [Arguments(null, null, "Production")]
    public async Task PhonePropertySet_SetsBoundPropertiesAsync(string? phoneNumber, string? simNumber, string status)
    {
        _phone.PhoneNumber = phoneNumber;
        _phone.SimNumber = simNumber;
        _phone.Status = status;
        var vm = _mocker.CreateInstance<PhonesItemViewModel>();

        await Assert.That(vm.AssetTag).IsEqualTo(_phone.AssetTag);
        await Assert.That(vm.FormerUser).IsEqualTo(_phone.FormerUser);
        await Assert.That(vm.Imei).IsEqualTo(_phone.Imei);
        await Assert.That(vm.Model).IsEqualTo(_phone.Model);
        await Assert.That(vm.NewUser).IsEqualTo(_phone.NewUser);
        await Assert.That(vm.NorR).IsEqualTo(_phone.Condition);
        await Assert.That(vm.Notes).IsEqualTo(_phone.Notes);
        await Assert.That(vm.OEM).IsEqualTo(_phone.OEM);
        await Assert.That(vm.PhoneNumber).IsEqualTo(_phone.PhoneNumber ?? string.Empty);
        await Assert.That(vm.SerialNumber).IsEqualTo(_phone.SerialNumber ?? string.Empty);
        await Assert.That(vm.SimNumber).IsEqualTo(_phone.SimNumber ?? string.Empty);
        await Assert.That(vm.SR).IsEqualTo(_phone.SR.ToString());
        await Assert.That(vm.Status).IsEqualTo(_phone.Status);
    }

    #region Update
    [Test]
    public async Task OnAssetTagChanged_CallsUpdateAsync_WithChangedValueAsync()
    {
        _vm.AssetTag = "Updated";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.AssetTag).IsEqualTo("Updated");
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("Former User", "Former User")]
    [Arguments("", null)]
    public async Task OnFormerUserChanged_CallsUpdateAsync_WithChangedValueAsync(string actual, string? expected)
    {
        _vm.FormerUser = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.FormerUser).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    public async Task OnModelChanged_CallsUpdateAsync_WithChangedValueAsync()
    {
        _vm.Model = "Updated";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.Model).IsEqualTo("Updated");
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("New User", "New User")]
    [Arguments("", null)]
    public async Task OnNewUserChanged_CallsUpdateAsync_WithChangedValueAsync(string actual, string? expected)
    {
        _vm.NewUser = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.NewUser).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);

    }

    [Test]
    public async Task OnNorRChanged_CallsUpdateAsync_WithChangedValueAsync()
    {
        _vm.NorR = "changed";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.Condition).IsEqualTo("changed");
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("New note", "New note")]
    [Arguments("", null)]
    public async Task OnNotesChanged_CallsUpdateAsync_WithChangedValueAsync(string actual, string? expected)
    {
        _vm.Notes = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.Notes).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    public async Task OnOEMChanged_CallsUpdateAsync_WithChangedValueAsync()
    {
        _vm.OEM = Manufacturer.Nokia;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.OEM).IsEqualTo(Manufacturer.Nokia);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("phone number", "phone number")]
    [Arguments("", null)]
    public async Task OnPhoneNumberChanged_CallsUpdateAsync_WithChangedValueAsync(string actual, string? expected)
    {
        _vm.PhoneNumber = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.PhoneNumber).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("serial number", "serial number")]
    [Arguments("", null)]
    public async Task OnSerialNumberChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.SerialNumber = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.SerialNumber).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("sim number", "sim number")]
    [Arguments("", null)]
    public async Task OnSimNumberChanged_CallsUpdateAsync_WithChangedValueAsync(string actual, string? expected)
    {
        _vm.SimNumber = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.SimNumber).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("345", 345)]
    [Arguments("", null)]
    public async Task OnSRChanged_CallsUpdateAsync_WithChangedValueAsync(string actual, int? expected)
    {
        _vm.SR = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.SR).IsEqualTo(expected);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    public async Task OnStatusChanged_CallsUpdateAsync_WithChangedValueAsync()
    {
        _vm.Status = "changed";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.Status).IsEqualTo("changed");
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("In Stock")]
    [Arguments("Decommissioned")]
    public async Task OnStatusChanged_ShouldClearNotes_WhenNewStatusInStockOrDecommissionedAsync(string status)
    {
        Mock<IApplicationSettingsRepository> settings = _mocker.GetMock<IApplicationSettingsRepository>();
        settings.Setup(s => s.ApplicationSettings).Returns(new ApplicationSettings());

        _vm.Status = status;

        await Assert.That(_vm.Notes).IsEmpty();
    }

    [Test]
    [Arguments("Decommissioned")]
    [Arguments("In Stock")]
    [Arguments("In Repair")]
    public async Task OnStatusChanged_ShouldClearProductionFields_WhenNewStatusInStockOrInRepairOrDecommissionedAsync(string status)
    {
        string? expectedFormerUser = _phone.NewUser;
        Mock<IApplicationSettingsRepository> settings = _mocker.GetMock<IApplicationSettingsRepository>();
        settings.Setup(s => s.ApplicationSettings).Returns(new ApplicationSettings());

        _vm.Status = status;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.DespatchDetails).IsNull();
        await Assert.That(_vm.FormerUser).IsEqualTo(expectedFormerUser);
        await Assert.That(_phone.NewUser).IsNull();
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("Awaiting Return")]
    [Arguments("Disposed")]
    [Arguments("Misplaced")]
    [Arguments("Production")]
    public async Task OnStatusChanged_ShouldKeepProductionFields_WhenNewStatusNotInStockOrInRepairOrDecommissionedAsync(string status)
    {
        _vm.Status = status;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        await Assert.That(_phone.DespatchDetails).IsEqualTo("despatch");
        await Assert.That(_vm.FormerUser).IsEqualTo(_phone.FormerUser);
        await Assert.That(_vm.NewUser).IsEqualTo(_phone.NewUser);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_phone.LastUpdate);
    }

    [Test]
    [Arguments("In Stock")]
    [Arguments("In Repair")]
    public async Task OnStatusChanged_ShouldClearTicketAsync(string status)
    {
        _vm.Status = status;

        await Assert.That(_vm.SR).IsNullOrEmpty();
    }

    [Test]
    [Arguments("Awaiting Return")]
    [Arguments("Disposed")]
    [Arguments("Misplaced")]
    [Arguments("Production")]
    public async Task OnStatusChanged_ShouldKeepTicketAsync(string status)
    {
        _vm.Status = status;

        await Assert.That(_vm.SR).IsEqualTo(_phone.SR.ToString());
    }

    [Test]
    public async Task OnStatusChanged_ShouldSetTicketToDefault_WhenDecommissionedAsync()
    {
        Mock<IApplicationSettingsRepository> settings = _mocker.GetMock<IApplicationSettingsRepository>();
        settings.Setup(s => s.ApplicationSettings).Returns(new ApplicationSettings() { DefaultDecommissionedTicket = 987654 });

        _vm.Status = "Decommissioned";

        await Assert.That(_vm.SR).IsEqualTo("987654");
    }
    #endregion

    #region RemoveSim
    [Test]
    public async Task RemoveSim_SetsBoundPropertiesAsync()
    {
        _vm.RemoveSimCommand.Execute(null);

        await Assert.That(_vm.PhoneNumber).IsEqualTo(string.Empty);
        await Assert.That(_vm.SimNumber).IsEqualTo(string.Empty);
        await Assert.That(_vm.LastUpdate).IsEqualTo(_updatedPhone.LastUpdate);
    }

    [Test]
    public async Task RemoveSimCommand_SetsCanExecute_FalseAsync()
    {
        _vm.RemoveSimCommand.Execute(null);

        await Assert.That(_vm.RemoveSimCommand.CanExecute(null)).IsFalse();
    }
    #endregion

    [Test]
    [Arguments("In Stock", null, null, false)]
    [Arguments("In Repair", null, null, false)]
    [Arguments("Production", null, null, false)]
    [Arguments("Production", 123, null, false)]
    [Arguments("Production", null, "new user", false)]
    [Arguments("Production", 123, "new user", true)]
    public async Task CreateEmailCommand_CanExecuteAsync(string status, int? sr, string? newUser, bool canExecute)
    {
        _phone.Status = status;
        _phone.SR = sr;
        _phone.NewUser = newUser;
        var vm = _mocker.CreateInstance<PhonesItemViewModel>();

        await Assert.That(vm.CreateEmailCommand.CanExecute(null)).IsEqualTo(canExecute);
    }
}
