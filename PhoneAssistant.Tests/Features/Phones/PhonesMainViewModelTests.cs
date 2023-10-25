using System.ComponentModel;
using System.Windows.Data;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Phones;

using Xunit;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesMainViewModelTests
{
    [Fact]
    public async Task RefreshPhonesCommand_AfterCRUDChanges_UpdatesViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2" , AssetTag = "Tag Bb2", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "321", AssetTag = "Tag Ccc3", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);
        index = 0;
        phones.Add(new v1Phone() { Imei = "444", AssetTag = "Tag ddd4", Model = "", NorR = "", OEM = "", Status = "" });

        vm.RefreshPhonesCommand.Execute(null);

        PhonesItemViewModel[] actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Equal(phones[3].Imei, actual[3].Imei);
    }

    [Fact]
    public async Task ChangingFilterAssetTag_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2" , AssetTag = "Tag Bb2", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", AssetTag = "Tag Ccc3", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterAssetTag = "B2";

        PhonesItemViewModel[] actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].AssetTag , actual[0].AssetTag);
    }

    [Fact]
    public async Task ChangingFilterFormerUser_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1" , FormerUser = "Aa", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "2", FormerUser = "Bbb", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", FormerUser = "Ccc", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterFormerUser = "BB";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].FormerUser, actual[0].FormerUser);
    }

    [Fact]
    public async Task ChangingFilterImei_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "11", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "22", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "33", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterImei = "22";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].Imei, actual[0].Imei);
    }

    [Fact]
    public async Task ChangingFilterNorR_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1", NorR="N", Model = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2", NorR="R", Model = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", NorR = "N", Model = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNorR = "R";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].NorR, actual[0].NorR);
    }

    [Fact]
    public async Task ChangingFilterNewUser_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1" , NewUser = "User Aa", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "2", NewUser = "User Bbb", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", NewUser = "User Ccc", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNewUser = "BB";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].NewUser, actual[0].NewUser);
    }

    [Fact]
    public async Task ChangingFilterNotes_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1" , Notes = "Note1", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2" , Notes = "Note2", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", Notes = "Note3", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNotes = "e2";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].Notes, actual[0].Notes);
    }

    [Fact]
    public async Task ChangingFilterOEM_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1" , OEM = "Apple", Model = "", NorR = "", Status = ""},
            new v1Phone() {Imei = "2", OEM = "Nokia", Model = "", NorR = "", Status = ""},
            new v1Phone() {Imei = "3", OEM = "Samsung", Model = "", NorR = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterOEM = "kia";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].OEM, actual[0].OEM);
    }

    [Fact]
    public async Task ChangingFilterPhoneNumber_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() {Imei = "1", PhoneNumber = "01", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2", PhoneNumber="02", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", PhoneNumber = "03", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterPhoneNumber = "02";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].PhoneNumber, actual[0].PhoneNumber);
    }

    [Fact]
    public async Task ChangingFilterSimNumber_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() {Imei = "1", SimNumber = "101", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2", SimNumber="202", Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", SimNumber = "303", Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSimNumber = "02";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].SimNumber, actual[0].SimNumber);
    }

    [Fact]
    public async Task ChangingFilterSR_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1", SR=111, Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() { Imei = "2", SR=222, Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "3", SR = 333, Model = "", NorR = "", OEM = "", Status = ""},
            new v1Phone() {Imei = "4", SR = 112233, Model = "", NorR = "", OEM = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSR = "22";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Equal(2, actual.Count());
        Assert.Equal(phones[1].SR.ToString(), actual[0].SR);
        Assert.Equal(phones[3].SR.ToString(), actual[1].SR);
    }

    [Fact]
    public async Task ChangingFilterStatus_ChangesFilterViewAsync()
    {
        List<v1Phone> phones = new List<v1Phone>() {
            new v1Phone() { Imei = "1", Status="Production", Model = "", NorR = "", OEM = ""},
            new v1Phone() { Imei = "2", Status="In Stock", Model = "", NorR = "", OEM = ""},
            new v1Phone() { Imei = "3", Status="In Repair", Model = "", NorR = "", OEM = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterStatus = "stock";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(phones[1].Status, actual[0].Status);
    }

    private int index = 0;
    private PhonesMainViewModel ViewModelMockSetup(List<v1Phone> phones)
    {
        AutoMocker mocker = new AutoMocker();

        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.GetPhonesAsync()).ReturnsAsync(phones);
        Mock<IPrintEnvelope> print = mocker.GetMock<IPrintEnvelope>();
        Mock<IPhonesItemViewModelFactory> factory = mocker.GetMock<IPhonesItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<v1Phone>()))
                            .Returns(() => {
                                return new PhonesItemViewModel(repository.Object, print.Object, phones[index]);
                                }
                            )
                            .Callback(() => index++);

        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();
        return vm;
    }
    //[TestMethod]
    //public void ViewModel_HasNoErrors_WhenIMEIvalid()
    //{
    //    IStateRepository stateRepository = Mock.Of<IStateRepository>();
    //    IPhonesRepository phonesRepository = Mock.Of<IPhonesRepository>();

    //    PhonesMainViewModel viewModel = new(phonesRepository, stateRepository)
    //    {
    //        Imei = "351554747259670"
    //    };

    //    Assert.IsFalse(viewModel.HasErrors);
    //}

    //[TestMethod]
    //[DataRow("a")]
    //[DataRow("351554747259671")]
    //[DataRow("8944125605563282810")]
    //public void ViewModel_HasErrors_WhenIMEIInvalid(string imei)
    //{
    //    IStateRepository stateRepository = Mock.Of<IStateRepository>();
    //    IPhonesRepository phonesRepository = Mock.Of<IPhonesRepository>();

    //    PhonesMainViewModel viewModel = new(phonesRepository, stateRepository)
    //    {
    //        Imei = imei
    //    };

    //    Assert.IsTrue(viewModel.HasErrors);
    //}

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
