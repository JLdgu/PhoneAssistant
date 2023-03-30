using Moq;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.ServiceRequests;

namespace PhoneAssistant.Tests.Features.ServiceRequests;

[TestClass]
public sealed class ServiceRequestsItemViewModelTests
{
    #region Constructor
    [TestMethod]
    public void Constructor_NewSR()
    {
        ServiceRequest expected = new() { Id = 0, ServiceRequestNumber = 0, NewUser = "", DespatchDetails = null };

        ServiceRequestsItemViewModel itemVM = new();        

        Assert.AreEqual(expected.ServiceRequestNumber, itemVM.ServiceRequestNumber);
        Assert.AreEqual(expected.NewUser, itemVM.NewUser);
        Assert.AreEqual(expected.DespatchDetails, itemVM.DespatchDetails);

        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsFalse(itemVM.CanCreateNewSR);
        Assert.IsFalse(itemVM.HasErrors);
    }

    [TestMethod]
    public void Constructor_ExistingSR()
    {
        ServiceRequest expected = new() { Id = 32, ServiceRequestNumber = 150032, NewUser = "U32", DespatchDetails = "D32" };

        ServiceRequestsItemViewModel itemVM = new(expected);

        Assert.AreEqual(expected.ServiceRequestNumber, itemVM.ServiceRequestNumber);
        Assert.AreEqual(expected.NewUser, itemVM.NewUser);
        Assert.AreEqual(expected.DespatchDetails, itemVM.DespatchDetails);

        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsTrue(itemVM.CanCreateNewSR);
        Assert.IsFalse(itemVM.HasErrors);
    }
    #endregion

    #region Save Changes
    [TestMethod]
    [DataRow(150000, "",null,"ServiceRequestNumber")]
    [DataRow(150000, "User 150000",null,"NewUser")]
    [DataRow(150000, "","Despatch 150000","DespatchDetails")]
    public void CanSaveSRChanges_WithValidSRChanges(int serviceRequestNumber, string newUser, string? despatchDetails, string property)
    {
        ServiceRequestsItemViewModel itemVM = new();

        itemVM.ServiceRequestNumber = serviceRequestNumber;
        itemVM.NewUser = newUser;
        itemVM.DespatchDetails = despatchDetails;

        Assert.IsTrue(itemVM.CanSaveSRChanges,property);
        Assert.IsFalse(itemVM.HasErrors, nameof(itemVM.HasErrors));
    }

    [TestMethod]
    [DataRow(149999, "", null, "ServiceRequestNumber")]
    [DataRow(1000000, "", null, "ServiceRequestNumber")]
    [DataRow(150000, "U", null, "NewUser")]
    public void CanSaveSRChanges_WithInvalidSRChanges(int serviceRequestNumber, string newUser, string? despatchDetails, string property)
    {
        ServiceRequestsItemViewModel itemVM = new();

        itemVM.ServiceRequestNumber = serviceRequestNumber;
        itemVM.NewUser = newUser;

        Assert.IsFalse(itemVM.CanSaveSRChanges, property);
        Assert.IsTrue(itemVM.HasErrors, nameof(itemVM.HasErrors));
    }
    #endregion

    #region Cancel 
    [TestMethod]
    [DataRow(150001, "", null, "ServiceRequestNumber")]
    [DataRow(150001, "User 150001", null, "NewUser")]
    [DataRow(150001, "", "Despatch 150001", "DespatchDetails")]
    public void CanCancelSRChanges_WithValidSRChanges(int serviceRequestNumber, string newUser, string? despatchDetails, string property)
    {
        ServiceRequestsItemViewModel itemVM = new();

        itemVM.ServiceRequestNumber = serviceRequestNumber;
        itemVM.NewUser = newUser;
        itemVM.DespatchDetails = despatchDetails;

        Assert.IsTrue(itemVM.CanCancelSRChanges,property);
        Assert.IsFalse(itemVM.HasErrors, nameof(itemVM.HasErrors));
    }

    [TestMethod]
    [DataRow(149999, "", null, "ServiceRequestNumber")]
    [DataRow(1000000, "", null, "ServiceRequestNumber")]
    [DataRow(150001, "U", null, "NewUser")]
    public void CanCancelSRChanges_WithInvalidSRChanges(int serviceRequestNumber, string newUser, string? despatchDetails, string property)
    {
        ServiceRequestsItemViewModel itemVM = new();

        itemVM.ServiceRequestNumber = serviceRequestNumber;
        itemVM.NewUser = newUser;

        Assert.IsTrue(itemVM.CanCancelSRChanges, property);
        Assert.IsTrue(itemVM.HasErrors, nameof(itemVM.HasErrors));
    }
    #endregion

    #region Create New SR
    [TestMethod]
    [DataRow(150002, "", null, "ServiceRequestNumber")]
    [DataRow(150002, "User 150002", null, "NewUser")]
    [DataRow(150002, "", "Despatch 150002", "DespatchDetails")]
    public void CanCreateNewSR_WithValidSRChanges(int serviceRequestNumber, string newUser, string? despatchDetails, string property)
    {
        ServiceRequestsItemViewModel itemVM = new();

        itemVM.ServiceRequestNumber = serviceRequestNumber;
        itemVM.NewUser = newUser;
        itemVM.DespatchDetails = despatchDetails;

        Assert.IsFalse(itemVM.CanCreateNewSR, property);
        Assert.IsFalse(itemVM.HasErrors, nameof(itemVM.HasErrors));
    }

    [TestMethod]
    [DataRow(149999, "", null, "ServiceRequestNumber")]
    [DataRow(1000000, "", null, "ServiceRequestNumber")]
    [DataRow(150002, "U", null, "NewUser")]
    public void CanCreateNewSR_WithinvalidSRChanges(int serviceRequestNumber, string newUser, string? despatchDetails, string property)
    {
        ServiceRequestsItemViewModel itemVM = new();

        itemVM.ServiceRequestNumber = serviceRequestNumber;
        itemVM.NewUser = newUser;

        Assert.IsFalse(itemVM.CanCreateNewSR, property);
        Assert.IsTrue(itemVM.HasErrors, nameof(itemVM.HasErrors));
    }
    #endregion
}
