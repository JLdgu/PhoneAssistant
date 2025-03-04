using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.Messaging;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesMainViewModelTests
{    
    [Test]
    public async Task Receive_ShouldAddPhoneAsync()
    {
        AutoMocker mocker = new();
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();

        vm.Receive(new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""});

        await Assert.That(vm.PhoneItems.Any()).IsTrue();
    }

    [Test]
    public async Task RefreshPhonesCommand_AfterCRUDChanges_UpdatesViewAsync()
    {
        List<Phone> phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = OEMs.Apple, Status = "In Stock"},
            new Phone() { Imei = "2" , AssetTag = "Tag B2", Model = "", Condition = "", OEM = OEMs.Samsung, Status = "In Repair"},
            new Phone() { Imei = "3" , AssetTag = "Tag C3", Model = "", Condition = "", OEM = OEMs.Nokia, Status = "Production"},
        ];

        PhonesMainViewModel vm = ViewModelMockSetup(phones, false);
        await vm.LoadAsync();

        vm.IncludeDisposals = false;

        Mock.VerifyAll();
    }

    [Test]
    public async Task IncludeDisposals_ShouldGetAllPhones_WhenTrue()
    {
        List<Phone> phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = OEMs.Apple, Status = "In Stock"},
            new Phone() { Imei = "2" , AssetTag = "Tag B2", Model = "", Condition = "", OEM = OEMs.Samsung, Status = "In Repair"},
            new Phone() { Imei = "3" , AssetTag = "Tag C3", Model = "", Condition = "", OEM = OEMs.Nokia, Status = "Production"},
            new Phone() { Imei = "4" , AssetTag = "Tag D4", Model = "", Condition = "", OEM = OEMs.Other, Status = "Decommissioned"},
            new Phone() { Imei = "5" , AssetTag = "Tag E5", Model = "", Condition = "", OEM = OEMs.Apple, Status = "Disposed"},
        ];
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();

        vm.IncludeDisposals = true;

        Mock.VerifyAll();
    }

    [Test]
    public async Task RefreshPhonesCommand_ShouldUpdateView_WhenDatabaseChanged()
    {
        List<Phone> phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2" , AssetTag = "Tag Bb2", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "3" , AssetTag = "Tag Ccc3", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        ];
        int index = 0;
        AutoMocker mocker = new();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.GetActivePhonesAsync()).ReturnsAsync(phones);
        Mock<IBaseReportRepository> sims = mocker.GetMock<IBaseReportRepository>();
        Mock<IUserSettings> settings = mocker.GetMock<IUserSettings>();
        Mock<IPrintEnvelope> print = mocker.GetMock<IPrintEnvelope>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        Mock<IPhonesItemViewModelFactory> factory = mocker.GetMock<IPhonesItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<Phone>()))
                            .Returns(() => new PhonesItemViewModel(repository.Object, sims.Object, settings.Object, print.Object, messenger.Object, phones[index]))
                            .Callback(() => index++);

        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();
        await vm.LoadAsync();
        index = 0;
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);
        phones.Add(new Phone() { Imei = "444", AssetTag = "Tag ddd4", Model = "", Condition = "", OEM = OEMs.Apple, Status = "" });

        vm.RefreshPhonesCommand.Execute(null);

        PhonesItemViewModel? actual = view.OfType<PhonesItemViewModel>().SingleOrDefault(vm => vm.Imei == "444");
        await Assert.That(actual).IsNotNull();
    }

    [Test]
    public async Task ChangingFilterAssetTag_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2" , AssetTag = "Tag Bb2", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", AssetTag = "Tag Ccc3", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterAssetTag = "B2";

        PhonesItemViewModel[] actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].AssetTag).IsEqualTo(phones[1].AssetTag );
    }

    [Test]
    public async Task ChangingFilterFormerUser_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , FormerUser = "Aa", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "2", FormerUser = "Bbb", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", FormerUser = "Ccc", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterFormerUser = "BB";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].FormerUser).IsEqualTo(phones[1].FormerUser);
    }

    [Test]
    public async Task ChangingFilterImei_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "11", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "22", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "33", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterImei = "22";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].Imei).IsEqualTo(phones[1].Imei);
    }

    [Test]
    public async Task ChangingFilterNorR_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1", Condition="N", Model = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2", Condition="R", Model = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", Condition = "N", Model = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNorR = "R";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].NorR).IsEqualTo(phones[1].Condition);
    }

    [Test]
    public async Task ChangingFilterNewUser_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , NewUser = "User Aa", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "2", NewUser = "User Bbb", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", NewUser = "User Ccc", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNewUser = "BB";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].NewUser).IsEqualTo(phones[1].NewUser);
    }

    [Test]
    public async Task ChangingFilterNotes_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , Notes = "Note1", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2" , Notes = "Note2", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", Notes = "Note3", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNotes = "e2";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].Notes).IsEqualTo(phones[1].Notes);
    }

    [Test]
    public async Task ChangingFilterOEM_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , OEM = OEMs.Apple, Model = "", Condition = "", Status = ""},
            new Phone() {Imei = "2", OEM = OEMs.Nokia, Model = "", Condition = "", Status = ""},
            new Phone() {Imei = "3", OEM = OEMs.Samsung, Model = "", Condition = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterOEM = OEMs.Samsung;

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].OEM).IsEqualTo(phones[2].OEM);
    }

    [Test]
    public async Task ChangingFilterPhoneNumber_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() {Imei = "1", PhoneNumber = "01", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2", PhoneNumber="02", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", PhoneNumber = "03", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterPhoneNumber = "02";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].PhoneNumber).IsEqualTo(phones[1].PhoneNumber);
    }

    [Test]
    public async Task ChangingFilterSimNumber_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() {Imei = "1", SimNumber = "101", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2", SimNumber="202", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", SimNumber = "303", Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSimNumber = "02";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].SimNumber).IsEqualTo(phones[1].SimNumber);
    }

    [Test]
    public async Task ChangingFilterSR_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1", SR=111, Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() { Imei = "2", SR=222, Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "3", SR = 333, Model = "", Condition = "", OEM = OEMs.Apple, Status = ""},
            new Phone() {Imei = "4", SR = 112233, Model = "", Condition = "", OEM = OEMs.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSR = "22";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual.Count()).IsEqualTo(2);
        await Assert.That(actual[0].SR).IsEqualTo(phones[1].SR.ToString());
        await Assert.That(actual[1].SR).IsEqualTo(phones[3].SR.ToString());
    }

    [Test]
    public async Task ChangingFilterStatus_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1", Status="Production", Model = "", Condition = "", OEM = OEMs.Apple},
            new Phone() { Imei = "2", Status="In Stock", Model = "", Condition = "", OEM = OEMs.Apple},
            new Phone() { Imei = "3", Status="In Repair", Model = "", Condition = "", OEM = OEMs.Apple}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterStatus = "stock";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].Status).IsEqualTo(phones[1].Status);
    }

    private PhonesMainViewModel ViewModelMockSetup(List<Phone> phones, bool getActive = true)
    {
        int index = 0;
        AutoMocker mocker = new();

        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        if (getActive ) 
            repository.Setup(r => r.GetActivePhonesAsync()).ReturnsAsync(phones);
        else
            repository.Setup(r => r.GetAllPhonesAsync()).ReturnsAsync(phones);
        Mock<IBaseReportRepository> sims = mocker.GetMock<IBaseReportRepository>();
        Mock<IUserSettings> settings = mocker.GetMock<IUserSettings>();
        Mock<IPrintEnvelope> print = mocker.GetMock<IPrintEnvelope>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        Mock<IPhonesItemViewModelFactory> factory = mocker.GetMock<IPhonesItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<Phone>()))
                            .Returns(() => new PhonesItemViewModel(repository.Object, sims.Object, settings.Object, print.Object, messenger.Object, phones[index]))
                            .Callback(() => index++);

        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();
        return vm;
    }
}
