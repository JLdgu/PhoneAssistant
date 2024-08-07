using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;
using Xunit.Abstractions;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class ReconciliationTests
{
    private readonly ITestOutputHelper _output;
    private readonly AutoMocker _mocker = new();
    private readonly Reconciliation _sut;

    public ReconciliationTests(ITestOutputHelper output)
    {
        _output = output;
        _sut = _mocker.CreateInstance<Reconciliation>();
    }

    [Fact]
    public async Task CheckStatus_ShouldReturn_WhenActionNotNull()
    {
        Mock<IDisposalsRepository> repository = _mocker.GetMock<IDisposalsRepository>();
        Disposal actual = new()
        {
            Imei = "355758060678186",
            StatusMS = null,
            StatusPA = ApplicationSettings.StatusDisposed,
            Manufacturer = "OEM",
            Model = "model",
            TrackedSKU = false,
            Action = "action"
        };

        await _sut.CheckStatusAsync(actual);

        Assert.Equal("action", actual.Action);
    }

    [Fact]
    public async Task CheckStatus_ShouldSetActionComplete_WhenMSandPADisposedAsync()
    {
        Disposal actual = new() { Imei = "355758060678186", StatusMS = ApplicationSettings.StatusDisposed, StatusPA = ApplicationSettings.StatusDisposed,
            Manufacturer = "OEMs", Model = "model", TrackedSKU = true};

        await _sut.CheckStatusAsync(actual);

        Assert.Equal(ReconciliationConstants.Complete, actual.Action);
    }

    [Fact]
    public async Task CheckStatus_ShouldSetComplete_WhenMSNullPADisposedandSKUNotTracked()
    {
        Disposal actual = new() { Imei = "355758060678186", StatusMS = null, StatusPA = ApplicationSettings.StatusDisposed, 
            Manufacturer = "OEM", Model = "model", TrackedSKU = false};

        await _sut.CheckStatusAsync(actual);

        Assert.Equal(ReconciliationConstants.Complete, actual.Action);
        _mocker.VerifyAll();
    }

    [Fact]
    public async Task CheckStatus_ShouldSetActionInvalid_WhenImeiInvalidAsync()
    {
        Disposal actual = new() { Imei = "355758060678286", StatusMS = null, StatusPA = null, Manufacturer = "OEM", Model = "model", TrackedSKU = true};

        await _sut.CheckStatusAsync(actual);

        Assert.Equal(ReconciliationConstants.ImeiInvalid, actual.Action);
    }

    [Theory]
    [InlineData(ApplicationSettings.StatusAwaitingReturn, ReconciliationConstants.MyScomisExport)]
    public async Task CheckStatus_ShouldSetActionMSDisposedAsync(string statusMS, string expectedAction)
    {
        Disposal actual = new() { Imei = "355758060678186", StatusMS = statusMS, StatusPA = null, Manufacturer = "OEM", Model = "model", TrackedSKU = true };

        await _sut.CheckStatusAsync(actual);

        Assert.Equal(expectedAction, actual.Action);
    }

    [Fact]
    public async Task CheckStatus_ShouldThrowException_WhenDisposalNullAsync()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CheckStatusAsync(null));        
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    //[Theory]
    //[InlineData(ApplicationSettings.StatusAwaitingReturn, ApplicationSettings.StatusDecommissioned, "Update myScomis status to Disposed")]
    //[InlineData(ApplicationSettings.StatusAwaitingReturn, ApplicationSettings.StatusDisposed, "Update myScomis status to Disposed")]
    //[InlineData(ApplicationSettings.StatusAwaitingReturn, ApplicationSettings.StatusInStock, "Update myScomis status to Disposed")]
    //[InlineData(ApplicationSettings.StatusAwaitingReturn, ApplicationSettings.StatusProduction, "Update myScomis status to Disposed")]
    //[InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDecommissioned, "Update phone status in myScomis")]
    //[InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDisposed, "Update myScomis status to Disposed")]
    //[InlineData(ApplicationSettings.StatusDisposed, null, "Add phone to PhoneAssistant")]
    //[InlineData(ApplicationSettings.StatusInStock, ApplicationSettings.StatusInStock, null)]
    //[InlineData(ApplicationSettings.StatusInStock, null, "Phone needs to be logged in PhoneAssistant")]
    //[InlineData(ApplicationSettings.StatusLost, null, "Update myScomis status to Disposed")]
    //[InlineData(ApplicationSettings.StatusProduction, ApplicationSettings.StatusProduction, null)]
    //[InlineData(ApplicationSettings.StatusProduction, null, null)]
    //[InlineData(ApplicationSettings.StatusUnlocated, null, "Update myScomis status to Disposed")]
    //[InlineData(null, ApplicationSettings.StatusDisposed, null)]
    //[InlineData(null, null, "Check CI linked to Disposal SR or add IMEI to Phones")]
    //public void CheckStatus_ShouldReturnAction_WhenMismatch(string? statusMS, string? statusPA, string? expectedAction)
    //{
    //    Disposal actual = new () { Imei = "355758060678186", StatusMS = statusMS, StatusPA = statusPA, OEM = OEMs.Other, Model = "model" };

    //    _sut.CheckStatus(actual);

    //    Assert.Equal(expectedAction, actual.Action);
    //}

    //[Theory]
    //[InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDecommissioned, "Update phone status in myScomis")]
    //[InlineData(ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDecommissioned, "Complete")]
    //public void CheckStatus_ShouldUpdatePhoneStatus_WhenSCCDisposed(string? statusMS, string? statusPA, string? expectedAction)
    //{
    //    Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
    //    repository.Setup(r => r.UpdateStatusAsync("355758060678186", ApplicationSettings.StatusDisposed));

    //    Disposal actual = new() { Imei = "355758060678186", StatusMS = statusMS, StatusPA = statusPA, OEM = OEMs.Other, Model = "model" };

    //    _sut.CheckStatus(actual);

    //    Assert.Equal(ApplicationSettings.StatusDisposed, actual.StatusPA);
    //    Assert.Equal(expectedAction, actual.Action);
    //    _mocker.VerifyAll();
    //}
}
