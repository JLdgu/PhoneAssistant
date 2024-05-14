using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class ReconciliationTests
{
    [Fact]
    private void PAUpdate_ShouldReturnAction_WhenMSandPAMismatch()
    {
        Disposal actual = new () { Imei = "imei", StatusMS = "Production", StatusPA = "In Stock"};

        Reconciliation.PAUpdate(actual);

        Assert.Equal("Reconcile", actual.Action);
    }
}
