﻿using Moq;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;
using PhoneAssistant.WPF.Application.Repositories;
using FluentAssertions;

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
        OEM = OEMs.Apple,
        SR = 123456
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
        OEM = OEMs.Apple,
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
        _repository.Setup(r => r.UpdateAsync(It.IsAny<Phone>()))
            .Callback<Phone>((p) => _phone = p);
        _repository.Setup(r => r.RemoveSimFromPhoneAsync(It.IsAny<Phone>()));
    }

    [Theory]
    [InlineData("phone number", "sim number","In Stock")]
    [InlineData(null, "sim number","In Stock")]
    [InlineData("phone number", null,"In Stock")]
    [InlineData(null, null, "In Stock")]
    [InlineData("phone number", "sim number",  "Production")]
    [InlineData(null, "sim number", "Production")]
    [InlineData("phone number", null, "Production")]
    [InlineData(null, null, "Production")]
    private void PhonePropertySet_SetsBoundProperties(string? phoneNumber, string? simNumber, string status)
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
        Assert.Equal(_phone.Condition, _vm.NorR);
        Assert.Equal(_phone.Notes, _vm.Notes);
        Assert.Equal(_phone.OEM, _vm.OEM);
        Assert.Equal(_phone.PhoneNumber ?? string.Empty, _vm.PhoneNumber);
        Assert.Equal(_phone.SimNumber ?? string.Empty, _vm.SimNumber);
        Assert.Equal(_phone.SR.ToString(), _vm.SR);
        Assert.Equal(_phone.Status, _vm.Status);
    }

    #region Update
    [Fact]
    public void OnAssetTagChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.AssetTag = "Updated";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("Updated", _phone.AssetTag);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("Former User", "Former User")]
    [InlineData("", null)]
    public void OnFormerUserChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.FormerUser = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.FormerUser);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Fact]
    public void OnModelChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.Model = "Updated";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("Updated", _phone.Model);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("New User", "New User")]
    [InlineData("", null)]
    public void OnNewUserChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.NewUser = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.NewUser);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);

    }

    [Fact]
    public void OnNorRChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.NorR = "changed";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("changed", _phone.Condition);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("New note", "New note")]
    [InlineData("", null)]
    public void OnNotesChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.Notes = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.Notes);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Fact]
    public void OnOEMChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.OEM = OEMs.Nokia;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(OEMs.Nokia, _phone.OEM);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("phone number", "phone number")]
    [InlineData("", null)]
    public void OnPhoneNumberChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.PhoneNumber = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.PhoneNumber);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("sim number", "sim number")]
    [InlineData("", null)]
    public void OnSimNumberChanged_CallsUpdateAsync_WithChangedValue(string actual, string? expected)
    {
        _vm.SimNumber = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.SimNumber);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("345", 345)]
    [InlineData("", null)]
    public void OnSRChanged_CallsUpdateAsync_WithChangedValue(string actual, int? expected)
    {
        _vm.SR = actual;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal(expected, _phone.SR);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Fact]
    public void OnStatusChanged_CallsUpdateAsync_WithChangedValue()
    {
        _vm.Status = "changed";

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("changed", _phone.Status);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("In Stock")]
    [InlineData("Decommissioned")]
    public void OnStatusChanged_ShouldClearNotes_WhenNewStatusInStockOrDecomissioned(string status)
    {
        string? expectedFormerUser = _phone.NewUser;
        _vm.Status = status;

        _vm.Notes.Should().BeEmpty();
    }

    [Theory]
    [InlineData("In Stock")]
    [InlineData("In Repair")]
    [InlineData("Decommissioned")]
    public void OnStatusChanged_ShouldClearProductionFields_WhenNewStatusInStockOrInRepairOrDecommissioned(string status)
    {
        string? expectedFormerUser = _phone.NewUser;
        _vm.Status = status;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Null(_phone.DespatchDetails);
        Assert.Equal(expectedFormerUser, _vm.FormerUser);
        Assert.Null(_phone.NewUser);
        Assert.Equal(string.Empty, _vm.SR);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("Awaiting Return")]
    [InlineData("Disposed")]
    [InlineData("Misplaced")]
    [InlineData("Production")]
    public void OnStatusChanged_ShouldKeepProductionFields_WhenNewStatusNotInStockOrInRepair(string status)
    {
        _vm.Status = status;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("despatch", _phone.DespatchDetails);
        Assert.Equal(_phone.FormerUser, _vm.FormerUser);
        Assert.Equal(_phone.NewUser, _vm.NewUser);
        //Assert.Equal(_phone.SR.ToString(), _vm.SR);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("Awaiting Return", "123454", "123454")]
    [InlineData("Decommissioned","123455", "")]
    [InlineData("Disposed","123456", "123456")]
    [InlineData("In Stock","123457", "")]
    [InlineData("In Repair","123458", "")]
    [InlineData("Misplaced","123459", "123459")]
    [InlineData("Production","123450", "123450")]
    public void OnStatusChanged_ShouldSetTicket(string status, string ticket, string expectedTicket)
    {        
        _vm.SR = ticket;

        _vm.Status = status;

        _vm.SR.Should().Be(expectedTicket);
    }

    #endregion

    #region RemoveSim
    [Fact]
    private void RemoveSim_SetsBoundProperties()
    {
        _vm.RemoveSimCommand.Execute(null);

        Assert.Equal(string.Empty, _vm.PhoneNumber);
        Assert.Equal(string.Empty, _vm.SimNumber);
        Assert.Equal(_updatedPhone.LastUpdate, _vm.LastUpdate);
    }

    [Fact]
    private void RemoveSimCommand_SetsCanExecute_False()
    {

        _vm.RemoveSimCommand.Execute(null);

        Assert.False(_vm.RemoveSimCommand.CanExecute(null));
    }
    #endregion

    [Theory]
    [InlineData("In Stock", null, null, false)]
    [InlineData("In Repair", null, null, false)]
    [InlineData("Production", null, null, false)]
    [InlineData("Production", 123, null, false)]
    [InlineData("Production", null, "new user", false)]
    [InlineData("Production", 123, "new user", true)]
    private void CreateEmailCommand_CanExecute(string status, int? sr, string? newUser, bool canExecute)
    {
        _phone.Status = status;
        _phone.SR = sr;
        _phone.NewUser = newUser;
        _vm = _mocker.CreateInstance<PhonesItemViewModel>();

        Assert.Equal(canExecute, _vm.CreateEmailCommand.CanExecute(null));
    }
}
