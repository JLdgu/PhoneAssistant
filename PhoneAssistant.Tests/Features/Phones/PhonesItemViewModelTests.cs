﻿using Moq;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;
using PhoneAssistant.WPF.Application.Repositories;
using FluentAssertions;
using PhoneAssistant.WPF.Application;

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
    public void PhonePropertySet_SetsBoundProperties(string? phoneNumber, string? simNumber, string status)
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
    [InlineData("Decommissioned")]
    [InlineData("In Stock")]
    [InlineData("In Repair")]
    public void OnStatusChanged_ShouldClearProductionFields_WhenNewStatusInStockOrInRepairOrDecommissioned(string status)
    {
        string? expectedFormerUser = _phone.NewUser;
        _vm.Status = status;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Null(_phone.DespatchDetails);
        Assert.Equal(expectedFormerUser, _vm.FormerUser);
        Assert.Null(_phone.NewUser);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("Awaiting Return")]
    [InlineData("Disposed")]
    [InlineData("Misplaced")]
    [InlineData("Production")]
    public void OnStatusChanged_ShouldKeepProductionFields_WhenNewStatusNotInStockOrInRepairOrDecommissioned(string status)
    {
        _vm.Status = status;

        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
        Assert.Equal("despatch", _phone.DespatchDetails);
        Assert.Equal(_phone.FormerUser, _vm.FormerUser);
        Assert.Equal(_phone.NewUser, _vm.NewUser);
        _vm.LastUpdate.Should().Be(_phone.LastUpdate);
    }

    [Theory]
    [InlineData("In Stock")]
    [InlineData("In Repair")]
    public void OnStatusChanged_ShouldClearTicket(string status)
    {
        _vm.Status = status;

        _vm.SR.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("Awaiting Return")]
    [InlineData("Disposed")]
    [InlineData("Misplaced")]
    [InlineData("Production")]
    public void OnStatusChanged_ShouldKeepTicket(string status)
    {
        _vm.Status = status;

        _vm.SR.Should().Be(_phone.SR.ToString());
    }

    [Fact]
    public void OnStatusChanged_ShouldSetTicketToDefault_WhenDecommissioned()
    {
        Mock<IUserSettings> settings;
        settings = _mocker.GetMock<IUserSettings>();
        settings.Setup(r => r.DefaultDecommissionedTicket).Returns(987654);

        _vm.Status = "Decommissioned";

        _vm.SR.Should().Be("987654");
    }
    #endregion

    #region RemoveSim
    [Fact]
    public void RemoveSim_SetsBoundProperties()
    {
        _vm.RemoveSimCommand.Execute(null);

        Assert.Equal(string.Empty, _vm.PhoneNumber);
        Assert.Equal(string.Empty, _vm.SimNumber);
        Assert.Equal(_updatedPhone.LastUpdate, _vm.LastUpdate);
    }

    [Fact]
    public void RemoveSimCommand_SetsCanExecute_False()
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
