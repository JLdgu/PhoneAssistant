using Moq;
using Moq.AutoMock;

using PhoneAssistant.Model;
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
    public void CheckStatus_ShouldReturn_WhenActionNotNull()
    {
        //Mock<IDisposalsRepository> repository = _mocker.GetMock<IDisposalsRepository>();
        Disposal actual = new()
        {
            Imei = "355758060678186",
            StatusPA = ApplicationConstants.StatusDisposed,
            Manufacturer = "OEM",
            Model = "model",
            TrackedSKU = false,
            Action = "action"
        };

        _sut.CheckStatus(actual);

        Assert.Equal("action", actual.Action);
    }

    [Fact]
    public void CheckStatus_ShouldSetActionComplete_WhenMSandPADisposed()
    {
        Disposal actual = new() { Imei = "355758060678186", StatusMS = ApplicationConstants.StatusDisposed, StatusPA = ApplicationConstants.StatusDisposed,
            Manufacturer = "OEMs", Model = "model", TrackedSKU = true};

        _sut.CheckStatus(actual);

        Assert.Equal(ReconciliationConstants.Complete, actual.Action);
    }

    [Fact]
    public void CheckStatus_ShouldSetActionComplete_WhenMSMissingPADisposedandSKUNotTracked()
    {
        Disposal actual = new() { Imei = "355758060678186", StatusPA = ApplicationConstants.StatusDisposed, 
            Manufacturer = "OEM", Model = "model", TrackedSKU = false};

        _sut.CheckStatus(actual);

        Assert.Equal(ReconciliationConstants.Complete, actual.Action);
        _mocker.VerifyAll();
    }

    [Fact]
    public void CheckStatus_ShouldSetActionInvalid_WhenImeiInvalid()
    {
        Disposal actual = new() { Imei = "355758060678286", StatusPA = null, Manufacturer = "OEM", Model = "model", TrackedSKU = true};

        _sut.CheckStatus(actual);

        Assert.Equal(ReconciliationConstants.ImeiInvalid, actual.Action);
    }

    [Theory]
    [InlineData(ApplicationConstants.StatusAwaitingReturn, ReconciliationConstants.MyScomisExport)]
    public void CheckStatus_ShouldSetActionMSDisposedAsync(string statusMS, string expectedAction)
    {
        Disposal actual = new() { Imei = "355758060678186", StatusMS = statusMS, StatusPA = null, Manufacturer = "OEM", Model = "model", TrackedSKU = true };

        _sut.CheckStatus(actual);

        Assert.Equal(expectedAction, actual.Action);
    }

    [Fact]
    public void CheckStatus_ShouldThrowException_WhenDisposalNullAsync()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => _sut.CheckStatus(null));        
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
