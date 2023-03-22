using Moq;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Phones;

[TestClass]
public sealed class PhonesMainViewModelTests
{
    [TestMethod]
    public void OnSelectedPhoneChanging_CallsUpdateAsync_WhenOutstandingChanges()
    {
        Phone selctedPhone = new Phone(id: 11, iMEI: "11", formerUser: "11", wiped: true, status: "In Stock", oEM: "Samsung", assetTag: null, note: null );
        Phone newSelctedPhone = new Phone(id: 999, iMEI: "99", formerUser: "99", wiped: true, status: "In Stock", oEM: "Samsung", assetTag: null, note: null);
        IStateRepository stateRepository = Mock.Of<IStateRepository>();
        var phonesRepository = new Mock<IPhonesRepository>();
        phonesRepository.Setup(pr => pr.UpdateAsync(selctedPhone));

        PhonesMainViewModel viewModel = new(phonesRepository.Object, stateRepository);

        viewModel.SelectedPhone = selctedPhone;
        viewModel.SelectedPhone = newSelctedPhone;

        phonesRepository.Verify(pr => pr.UpdateAsync(selctedPhone), Times.Once);
    }

    [TestMethod]
    public void OnSelectedPhoneChanging_DoesNotCallsUpdateAsync_WhenNoOutstandingChanges()
    {
        Phone selctedPhone = new Phone(id: 11, iMEI: "11", formerUser: "11", wiped: true, status: "In Stock", oEM: "Samsung", assetTag: null, note: null );        
        IStateRepository stateRepository = Mock.Of<IStateRepository>();
        var phonesRepository = new Mock<IPhonesRepository>();
        phonesRepository.Setup(pr => pr.UpdateAsync(selctedPhone));

        PhonesMainViewModel viewModel = new(phonesRepository.Object, stateRepository);

        viewModel.SelectedPhone = selctedPhone;

        phonesRepository.Verify(pr => pr.UpdateAsync(selctedPhone), Times.Never);
    }

    [TestMethod]
    public async Task WindowClosingAsync_CallsUpdateAsync_WhenPossibleOutstandingChanges()
    {
        Phone selctedPhone = new Phone(id: 11, iMEI: "11", formerUser: "11", wiped: true, status: "In Stock", oEM: "Samsung", assetTag: null, note: null );
        Phone newSelctedPhone = new Phone(id: 999, iMEI: "99", formerUser: "99", wiped: true, status: "In Stock", oEM: "Samsung", assetTag: null, note: null);        
        IStateRepository stateRepository = Mock.Of<IStateRepository>();
        var phonesRepository = new Mock<IPhonesRepository>();
        phonesRepository.Setup(pr => pr.UpdateAsync(selctedPhone));

        PhonesMainViewModel viewModel = new(phonesRepository.Object, stateRepository);

        viewModel.SelectedPhone = selctedPhone;
        await viewModel.WindowClosingAsync();

        phonesRepository.Verify(pr => pr.UpdateAsync(selctedPhone), Times.Once);
    }    
}
