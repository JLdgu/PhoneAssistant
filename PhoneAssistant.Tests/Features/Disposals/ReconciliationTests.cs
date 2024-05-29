using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class ReconciliationTests
{
    [Fact]
    public void Execute_ShouldThrowException_WhenDisposalNull()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => Reconciliation.CheckStatus(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Theory]
    [InlineData(ApplicationSettings.StatusAwaitingReturn, null, null, null)]
    [InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDecommissioned, null, null)]
    [InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDisposed, "Update phone status in myScomis and PhoneAssistant")]
    [InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, "Update myScomis status to Disposed")]
    [InlineData(ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, null)]
    [InlineData(ApplicationSettings.StatusDisposed, null, ApplicationSettings.StatusDisposed, "Add phone to PhoneAssistant")]
    [InlineData(ApplicationSettings.StatusDisposed, null, null, "Check if phone is an SCC disposal")]
    [InlineData(ApplicationSettings.StatusInStock, ApplicationSettings.StatusInStock, null, null)]
    [InlineData(ApplicationSettings.StatusInStock, null, null, "Phone needs to be logged in PhoneAssistant")]
    [InlineData(ApplicationSettings.StatusLost, null, null, null)]
    [InlineData(ApplicationSettings.StatusProduction, ApplicationSettings.StatusProduction, null, null)]
    [InlineData(ApplicationSettings.StatusProduction, null, null, null)]
    [InlineData(ApplicationSettings.StatusUnlocated, null, null, null)]
    [InlineData(null, ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, null)]
    [InlineData(null, null, ApplicationSettings.StatusDisposed, "Check CI linked to Disposal SR or add IMEI to Phones")]
    public void Execute_ShouldReturnAction_WhenMismatch(string? statusMS, string? statusPA, string? statusSCC, string? expectedAction)
    {
        Disposal actual = new () { Imei = "imei", StatusMS = statusMS, StatusPA = statusPA, StatusSCC = statusSCC};

        Reconciliation.CheckStatus(actual);

        Assert.Equal(expectedAction, actual.Action);
    }
}
