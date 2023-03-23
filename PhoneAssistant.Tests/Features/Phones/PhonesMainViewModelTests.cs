using Moq;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Features.Phones;

[TestClass]
public sealed class PhonesMainViewModelTests
{
    [TestMethod]
    public void ViewModel_HasNoErrors_WhenIMEIvalid()
    {
        IStateRepository stateRepository = Mock.Of<IStateRepository>();
        IPhonesRepository phonesRepository = Mock.Of<IPhonesRepository>();

        PhonesMainViewModel viewModel = new(phonesRepository, stateRepository);

        viewModel.IMEI = "351554747259670";

        Assert.IsFalse(viewModel.HasErrors);
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("351554747259671")]
    [DataRow("8944125605563282810")]
    public void ViewModel_HasErrors_WhenIMEIInvalid(string imei)
    {
        IStateRepository stateRepository = Mock.Of<IStateRepository>();
        IPhonesRepository phonesRepository = Mock.Of<IPhonesRepository>();

        PhonesMainViewModel viewModel = new(phonesRepository, stateRepository);
        viewModel.IMEI = imei;

        Assert.IsTrue(viewModel.HasErrors);
    }

    //[TestMethod]
    //public void OnSelectedPhoneChanging_CallsUpdateAsync_WhenOutstandingChanges()
    //{
    //    Phone selctedPhone = new Phone() { Id = 11, IMEI = "11", FormerUser = "11", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };
    //    Phone newSelctedPhone = new Phone() { Id = 999, IMEI = "99", FormerUser = "99", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };
    //    IStateRepository stateRepository = Mock.Of<IStateRepository>();
    //    var phonesRepository = new Mock<IPhonesRepository>();
    //    phonesRepository.Setup(pr => pr.UpdateAsync(selctedPhone));

    //    PhonesMainViewModel viewModel = new(phonesRepository.Object, stateRepository);

    //    viewModel.SelectedPhone = selctedPhone;
    //    viewModel.SelectedPhone = newSelctedPhone;

    //    phonesRepository.Verify(pr => pr.UpdateAsync(selctedPhone), Times.Once);
    //}

    //[TestMethod]
    //public void OnSelectedPhoneChanging_DoesNotCallsUpdateAsync_WhenNoOutstandingChanges()
    //{
    //    Phone selctedPhone = new Phone() { Id = 11, IMEI = "11", FormerUser = "11", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };
    //    IStateRepository stateRepository = Mock.Of<IStateRepository>();
    //    var phonesRepository = new Mock<IPhonesRepository>();
    //    phonesRepository.Setup(pr => pr.UpdateAsync(selctedPhone));

    //    PhonesMainViewModel viewModel = new(phonesRepository.Object, stateRepository);

    //    viewModel.SelectedPhone = selctedPhone;

    //    phonesRepository.Verify(pr => pr.UpdateAsync(selctedPhone), Times.Never);
    //}

    //[TestMethod]
    //public async Task WindowClosingAsync_CallsUpdateAsync_WhenPossibleOutstandingChanges()
    //{
    //    Phone selctedPhone = new Phone() { Id = 11, IMEI = "11", FormerUser = "11", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };
    //    Phone newSelctedPhone = new Phone() { Id = 999, IMEI = "99", FormerUser = "99", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };
    //    IStateRepository stateRepository = Mock.Of<IStateRepository>();
    //    var phonesRepository = new Mock<IPhonesRepository>();
    //    phonesRepository.Setup(pr => pr.UpdateAsync(selctedPhone));

    //    PhonesMainViewModel viewModel = new(phonesRepository.Object, stateRepository);

    //    viewModel.SelectedPhone = selctedPhone;
    //    await viewModel.WindowClosingAsync();

    //    phonesRepository.Verify(pr => pr.UpdateAsync(selctedPhone), Times.Once);
    //}    
}
