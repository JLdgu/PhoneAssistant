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
        Assert.Throws<ArgumentNullException>(() => Reconciliation.CheckStatus(null));
    }

    [Theory]
    [InlineData(ApplicationSettings.StatusAwaitingReturn, null, null, null)]
    [InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDecommissioned, null, null)]
    [InlineData(ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDecommissioned, ApplicationSettings.StatusDisposed, "Update phone status in myScomis and PhoneAssistant")]
    [InlineData(ApplicationSettings.StatusDisposed, null, null, "Check if phone is an SCC disposal")]
    [InlineData(ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, null)]
    [InlineData(ApplicationSettings.StatusInStock, null, null, "Phone needs to be logged in PhoneAssistant")]
    [InlineData(ApplicationSettings.StatusInStock, ApplicationSettings.StatusInStock, null, null)]
    [InlineData(ApplicationSettings.StatusLost, null, null, null)]
    [InlineData(ApplicationSettings.StatusProduction, null, null, null)]
    [InlineData(ApplicationSettings.StatusProduction, ApplicationSettings.StatusProduction, null, null)]
    [InlineData(ApplicationSettings.StatusUnlocated, null, null, null)]

    //[InlineData(null, ApplicationSettings.StatusDecommissioned, null, "Phone CI missing from myScomis")]
    //[InlineData(null, ApplicationSettings.StatusDisposed, null, null)]
    //[InlineData(null, ApplicationSettings.StatusInRepair, null, "Phone CI missing from myScomis")]
    //[InlineData(null, ApplicationSettings.StatusInStock, null, "Phone CI missing from myScomis")]
    //[InlineData(null, ApplicationSettings.StatusProduction, null, "Phone CI missing from myScomis")]
    //[InlineData(ApplicationSettings.StatusDisposed, ApplicationSettings.StatusDisposed, null, null)]    
    //[InlineData(ApplicationSettings.StatusProduction, ApplicationSettings.StatusInStock,null, "Reconcile")]
    //[InlineData("invalid status", ApplicationSettings.StatusInStock, null, "No matching reconcilation rule")]
    //[InlineData(ApplicationSettings.StatusProduction, "invalid status", null, "No matching reconcilation rule")]    
    public void Execute_ShouldReturnAction_WhenMismatch(string? statusMS, string? statusPA, string? statusSCC, string? expectedAction)
    {
        Disposal actual = new () { Imei = "imei", StatusMS = statusMS, StatusPA = statusPA, StatusSCC = statusSCC};

        Reconciliation.CheckStatus(actual);

        Assert.Equal(expectedAction, actual.Action);
    }
}
