using Moq;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.ServiceRequests;

namespace PhoneAssistant.Tests.Features.ServiceRequests;

[TestClass]
public sealed class ServiceRequestsItemViewModelTests
{
    #region CanCancelSRChanges
    [TestMethod]
    public void CanCancelSRChanges_False_ForNewSR_WithNoChanges()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);

        Assert.IsFalse(itemVM.CanCancelSRChanges);
    }

    [TestMethod]
    public void CanCancelSRChanges_False_ForExistingSR_WithNoChanges()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 999999, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;

        Assert.IsFalse(itemVM.CanCancelSRChanges);
    }

    [TestMethod]
    [DataRow(160000, null, null)]
    [DataRow(null, "User", null)]
    [DataRow(null, null, "Despatch")]
    [DataRow(160000, "User", null)]
    [DataRow(160000, null, "Despatch")]
    [DataRow(null, "User", "Despatch")]
    [DataRow(160000, "User", "Despatch")]
    public void CanCancelSRChanges_True_ForNewSR_WithValidChanges(int? srn, string? user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);

        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;
        if (despatch is not null)
            itemVM.DespatchDetails = despatch;

        Assert.IsTrue(itemVM.CanCancelSRChanges);
    }

    [TestMethod]
    [DataRow(160000, null, null)]
    [DataRow(null, "User", null)]
    [DataRow(null, null, "Despatch")]
    [DataRow(160000, "User", null)]
    [DataRow(160000, null, "Despatch")]
    [DataRow(null, "User", "Despatch")]
    [DataRow(160000, "User", "Despatch")]
    public void CanCancelSRChanges_True_ForExistingSR_WithValidChanges(int? srn, string? user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 999999, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;

        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;
        if (despatch is not null)
            itemVM.DespatchDetails = despatch;

        Assert.IsTrue(itemVM.CanCancelSRChanges);
    }
    #endregion

    #region CanCreateNewSR
    [TestMethod]
    public void CanCreateNewSR_False_ForNewSR_WithNoChanges()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);

        Assert.IsFalse(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    [DataRow(160000, null, null)] // Valid Changes
    [DataRow(null, "User", null)]
    [DataRow(null, null, "Despatch")]
    [DataRow(160000, "User", null)]
    [DataRow(160000, null, "Despatch")]
    [DataRow(null, "User", "Despatch")]
    [DataRow(160000, "User", "Despatch")]
    [DataRow(149999, null, null)] // Invalid changes
    [DataRow(1000000, null, null)]
    [DataRow(null, "I", null)]
    [DataRow(null, "Valid", "I")]
    public void CanCreateNewSR_False_ForNewSR_WithChanges(int? srn, string? user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;
        if (despatch is not null)
            itemVM.DespatchDetails = despatch;

        Assert.IsFalse(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    [DataRow(160000, null, null)] // Valid Changes
    [DataRow(null, "User", null)]
    [DataRow(null, null, "Despatch")]
    [DataRow(160000, "User", null)]
    [DataRow(160000, null, "Despatch")]
    [DataRow(null, "User", "Despatch")]
    [DataRow(160000, "User", "Despatch")]
    [DataRow(149999, null, null)] // Invalid changes
    [DataRow(1000000, null, null)]
    [DataRow(null, "I", null)]
    [DataRow(null, "Valid", "I")]
    public void CanCreateNewSR_False_ForExistingSR_WithChanges(int? srn, string? user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 500000, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;
        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;
        if (despatch is not null)
            itemVM.DespatchDetails = despatch;

        Assert.IsFalse(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    public void CanCreateNewSR_True_ForExistingSR_WithNoChanges()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest expected = new() { Id = 32, ServiceRequestNumber = 150032, NewUser = "U32", DespatchDetails = "D32" };

        itemVM.SR = expected;

        Assert.IsTrue(itemVM.CanCreateNewSR);
    }
    #endregion

    #region CanSaveSRChanges
    [TestMethod]
    [DataRow(149999, null, null)]
    [DataRow(1000000, null, null)]
    [DataRow(null, "I", null)]
    [DataRow(null, "Valid", "I")]
    public void CanSaveSRChanges_False_ForNewSR_WithInvalidChanges(int? srn, string? user1, string? user2)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user1 is not null)
            itemVM.NewUser = user1;
        if (user2 is not null)
            itemVM.NewUser = user2;

        Assert.IsFalse(itemVM.CanSaveSRChanges);
    }

    [TestMethod]
    public void CanSaveSRChanges_False_ForExistingSR_WithInvalidChanges()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 999999, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;

        Assert.IsFalse(itemVM.CanCancelSRChanges);
    }

    [TestMethod]
    [DataRow(149999, null)]
    [DataRow(1000000, null)]
    [DataRow(null, "I")]
    public void CanSaveSRChanges_False_ForExistingSR_WithInvalidChanges(int? srn, string? user)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 500000, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;
        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;

        Assert.IsFalse(itemVM.CanCancelSRChanges);
    }
    #endregion

    #region HasErrors
    [TestMethod]
    [DataRow(null, null, null)]
    [DataRow(160000, null, null)]
    [DataRow(null, "User", null)]
    [DataRow(null, null, "Despatch")]
    [DataRow(160000, "User", null)]
    [DataRow(160000, null, "Despatch")]
    [DataRow(null, "User", "Despatch")]
    [DataRow(160000, "User", "Despatch")]
    public void HasErrors_False_ForNewSR_WithValidChanges(int? srn, string? user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);

        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;
        if (despatch is not null)
            itemVM.DespatchDetails = despatch;

        Assert.IsFalse(itemVM.HasErrors);
    }

    [TestMethod]
    [DataRow(null, null, null)]
    [DataRow(160000, null, null)]
    [DataRow(null, "User", null)]
    [DataRow(null, null, "Despatch")]
    [DataRow(160000, "User", null)]
    [DataRow(160000, null, "Despatch")]
    [DataRow(null, "User", "Despatch")]
    [DataRow(160000, "User", "Despatch")]
    public void HasErrors_False_ForExistingSR_WithValidChanges(int? srn, string? user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 999999, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;

        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;
        if (despatch is not null)
            itemVM.DespatchDetails = despatch;

        Assert.IsFalse(itemVM.HasErrors);
    }

    [TestMethod]
    [DataRow(149999, null, null)]
    [DataRow(1000000, null, null)]
    [DataRow(null, "I", null)]
    [DataRow(null, "Valid", "I")]
    public void HasErrors_True_ForNewSR_WithInvalidChanges(int? srn, string? user1, string? user2)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user1 is not null)
            itemVM.NewUser = user1;
        if (user2 is not null)
            itemVM.NewUser = user2;

        Assert.IsTrue(itemVM.HasErrors);
    }

    [TestMethod]
    [DataRow(149999, null)]
    [DataRow(1000000, null)]
    [DataRow(null, "I")]
    public void HasErrors_True_ForExistingSR_WithInvalidChanges(int? srn, string? user)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest existingSR = new() { Id = 1234, ServiceRequestNumber = 500000, NewUser = "Valid User", DespatchDetails = null };
        itemVM.SR = existingSR;
        if (srn is not null)
            itemVM.ServiceRequestNumber = (int)srn;
        if (user is not null)
            itemVM.NewUser = user;

        Assert.IsTrue(itemVM.HasErrors);
    }
    #endregion

    #region SaveSRChangesCommand
    [TestMethod]
    public void SaveSRChanges_UpdatesButtons_ForNewSR()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        itemVM.ServiceRequestNumber = 150001;
        itemVM.NewUser = "New User";

        Assert.IsTrue(itemVM.CanSaveSRChanges);
        itemVM.SaveSRChangesCommand.Execute(null);

        Assert.IsFalse(itemVM.HasErrors);
        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsTrue(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    public void SaveSRChanges_UpdatesButtons_ForExistingSR()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest expected = new() { Id = 1, ServiceRequestNumber = 500000, NewUser = "User", DespatchDetails = null };
        itemVM.SR = expected;

        itemVM.DespatchDetails = "Despatch";

        Assert.IsTrue(itemVM.CanSaveSRChanges);
        itemVM.SaveSRChangesCommand.Execute(null);

        Assert.IsFalse(itemVM.HasErrors);
        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsTrue(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    public void SaveSRChanges_CallsUpdateAsync_ForNewSR()
    {
        ServiceRequest expected = new() { Id = 0, ServiceRequestNumber = 150001, NewUser = "New User", DespatchDetails = null};
        var srRepository = new Mock<IServiceRequestsRepository>();
        srRepository.Setup(r => r.UpdateAsync(It.IsAny<ServiceRequest>()));
        ServiceRequestsItemViewModel itemVM = new(srRepository.Object);

        itemVM.ServiceRequestNumber = expected.ServiceRequestNumber;
        itemVM.NewUser = expected.NewUser;

        Assert.IsTrue(itemVM.CanSaveSRChanges);
        itemVM.SaveSRChangesCommand.Execute(null);

        srRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequest>()), Times.Once);
        Assert.IsFalse(itemVM.HasErrors);
        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsTrue(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    public void SaveSRChanges_CallsUpdateAsync_ForExistingSR()
    {
        ServiceRequest expected = new() { Id = 42, ServiceRequestNumber = 150000, NewUser = "User", DespatchDetails = null };
        var srRepository = new Mock<IServiceRequestsRepository>();
        srRepository.Setup(r => r.UpdateAsync(It.IsAny<ServiceRequest>()));
        ServiceRequestsItemViewModel itemVM = new(srRepository.Object);
        itemVM.SR = expected;

        //itemVM.ServiceRequestNumber = 150001;
        //itemVM.NewUser = "New User";
        itemVM.DespatchDetails = "Despatch";

        Assert.IsTrue(itemVM.CanSaveSRChanges);
        itemVM.SaveSRChangesCommand.Execute(null);

        srRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequest>()), Times.Once);
        Assert.IsFalse(itemVM.HasErrors);
        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsTrue(itemVM.CanCreateNewSR);
    }

    //[TestMethod]
    //[DataRow(32, 160000, "User", null)]
    //[DataRow(33, 160000, "User", "Despatch")]
    //public void SaveSRChanges_Raises_SRChanged(int id, int srn, string user, string? despatch)
    //{
    //    IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
    //    ServiceRequestsItemViewModel itemVM = new(srRepository);
    //    ServiceRequest expected = new() { Id = id, ServiceRequestNumber = srn, NewUser = user, DespatchDetails = despatch };
    //    itemVM.SR = expected;
    //    //expected.ServiceRequestNumber += 1;
    //    itemVM.ServiceRequestNumber += 1; //expected.ServiceRequestNumber;

    //    Assert.IsTrue(itemVM.CanSaveSRChanges);
    //    itemVM.SaveSRChangesCommand.Execute(null);

    //    Assert.Fail();
    //}
    #endregion

    #region Cancel Changes to SR 
    [TestMethod]
    public void CancelSRChanges_NewSR()
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest expected = new() { Id = 0, ServiceRequestNumber = 0, NewUser = "", DespatchDetails = null };

        itemVM.ServiceRequestNumber = 150000;
        Assert.IsTrue(itemVM.CanCancelSRChanges);

        itemVM.CancelSRChangesCommand.Execute(null);

        Assert.AreEqual(expected.ServiceRequestNumber, itemVM.ServiceRequestNumber);
        Assert.AreEqual(expected.NewUser, itemVM.NewUser);
        Assert.AreEqual(expected.DespatchDetails, itemVM.DespatchDetails);

        Assert.IsFalse(itemVM.HasErrors);
        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsFalse(itemVM.CanCreateNewSR);
    }

    [TestMethod]
    [DataRow(32, 160000, "User", null)]
    [DataRow(33, 160000, "User", "Despatch")]
    public void CancelSRChanges_ExistingSR(int id, int srn, string user, string? despatch)
    {
        IServiceRequestsRepository srRepository = Mock.Of<IServiceRequestsRepository>();
        ServiceRequestsItemViewModel itemVM = new(srRepository);
        ServiceRequest expected = new() { Id = id, ServiceRequestNumber = srn, NewUser = user, DespatchDetails = despatch };
        itemVM.SR = expected;
        itemVM.ServiceRequestNumber += 1;

        itemVM.CancelSRChangesCommand.Execute(null);

        Assert.AreEqual(srn, itemVM.ServiceRequestNumber);
        Assert.AreEqual(user, itemVM.NewUser);
        Assert.AreEqual(despatch, itemVM.DespatchDetails);

        Assert.IsFalse(itemVM.HasErrors);
        Assert.IsFalse(itemVM.CanSaveSRChanges);
        Assert.IsFalse(itemVM.CanCancelSRChanges);
        Assert.IsTrue(itemVM.CanCreateNewSR);
    }
    #endregion
}
