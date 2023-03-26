using Moq;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.ServiceRequests;

namespace PhoneAssistant.Tests.Features.ServiceRequests;

[TestClass]
public sealed class ServiceRequestsItemViewModelTests
{
    [TestMethod]
    public void Constructor_SetsVMProperties()
    {
        ServiceRequest expected = new() { Id = 32, ServiceRequestNumber = 32, NewUser = "U32", DespatchDetails = "D32" };

        ServiceRequestsItemViewModel itemVM = new(expected)
        {
            SR = expected
        };

        Assert.AreEqual(expected.ServiceRequestNumber, itemVM.ServiceRequestNumber);
        Assert.AreEqual(expected.NewUser, itemVM.NewUser);
        Assert.AreEqual(expected.DespatchDetails, itemVM.DespatchDetails); 
    }

    [TestMethod]
    public void OnSRChanged_SetsVMProperties()
    {
        ServiceRequest sr102 = new() { Id = 102, ServiceRequestNumber = 102, NewUser = "Test User 102", DespatchDetails="D102" };
        ServiceRequest expected = new() { Id = 33, ServiceRequestNumber = 33, NewUser = "U33" , DespatchDetails = "D33" };

        ServiceRequestsItemViewModel itemVM = new(sr102)
        {
            SR = expected
        };

        Assert.AreEqual(expected.ServiceRequestNumber, itemVM.ServiceRequestNumber);
        Assert.AreEqual(expected.NewUser, itemVM.NewUser);
        Assert.AreEqual(expected.DespatchDetails, itemVM.DespatchDetails);
    }
}
