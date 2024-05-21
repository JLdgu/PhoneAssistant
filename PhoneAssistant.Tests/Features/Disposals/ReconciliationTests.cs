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
        Assert.Throws<ArgumentNullException>(() => Reconciliation.Execute(null));
    }

    [Theory]
    [InlineData(null,ApplicationSettings.StatusDecommissioned,"Phone CI missing from myScomis")]
    [InlineData(null, ApplicationSettings.StatusDisposed, null)]
    [InlineData(null, ApplicationSettings.StatusInRepair, "Phone CI missing from myScomis")]
    [InlineData(null, ApplicationSettings.StatusInStock, "Phone CI missing from myScomis")]
    [InlineData(null, ApplicationSettings.StatusProduction, "Phone CI missing from myScomis")]
    [InlineData(ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, null)]
    [InlineData(ApplicationSettings.StatusProduction, ApplicationSettings.StatusInStock, "Reconcile")]
    [InlineData("invalid status", ApplicationSettings.StatusInStock, "No matching reconcilation rule")]
    [InlineData(ApplicationSettings.StatusProduction, "invalid status", "No matching reconcilation rule")]
    
    public void Execute_ShouldReturnAction_WhenMismatch(string? statusMS, string? statusPA, string? expectedAction)
    {
        Disposal actual = new () { Imei = "imei", StatusMS = statusMS, StatusPA = statusPA};

        Reconciliation.Execute(actual);

        Assert.Equal(expectedAction, actual.Action);
    }
}
