using Moq.AutoMock;
using Moq;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Sims;
using System.ComponentModel;
using System.Windows.Data;
using Xunit;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.Tests.Features.Sims;
public sealed class SimsMainViewModelTests
{
    [Fact]
    public async Task RefreshPhonesCommand_AfterCRUDChanges_UpdatesViewAsync()
    {
        List<Sim> sims = new List<Sim>()
        {
            new Sim() {PhoneNumber = "1", SimNumber = "1.1", AssetTag = "B1"},
            new Sim() {PhoneNumber = "2", SimNumber = "2.2", AssetTag = "B2"},
            new Sim() {PhoneNumber = "3", SimNumber = "3.3"}
        };
        SimsMainViewModel vm = ViewModelMockSetup(sims);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.SimItems);
        index = 0;
        sims.Add(new Sim() { PhoneNumber = "4", SimNumber = "4.4", AssetTag = "D4" });

        vm.RefreshSimsCommand.Execute(null);

        var actual = view.OfType<SimsItemViewModel>().ToArray();
        Assert.Equal(sims[3].PhoneNumber, actual[3].PhoneNumber);
    }

    [Fact]
    public async Task ChangingFilterAssetTag_ChangesFilterViewAsync()
    {
        List<Sim> sims = new List<Sim>()
        {
            new Sim() {PhoneNumber = "1", SimNumber = "1.1", AssetTag = "B1"},
            new Sim() {PhoneNumber = "2", SimNumber = "2.2", AssetTag = "B2"},
            new Sim() {PhoneNumber = "3", SimNumber = "3.3"}
        };
            
        SimsMainViewModel vm = ViewModelMockSetup(sims);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.SimItems);

        vm.FilterAssetTag = "B2";

        var actual = view.OfType<SimsItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(sims[1].AssetTag, actual[0].AssetTag);
    }

    [Fact]
    public async Task ChangingFilterNotes_ChangesFilterViewAsync()
    {
        List<Sim> sims = new List<Sim>()
        {
            new Sim() {PhoneNumber = "1", SimNumber = "1.1", Notes = "B1"},
            new Sim() {PhoneNumber = "2", SimNumber = "2.2", Notes = "B2"},
            new Sim() {PhoneNumber = "3", SimNumber = "3.3"}
        };

        SimsMainViewModel vm = ViewModelMockSetup(sims);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.SimItems);

        vm.FilterNotes = "B2";

        var actual = view.OfType<SimsItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(sims[1].Notes, actual[0].Notes);
    }

    [Fact]
    public async Task ChangingFilterPhoneNumber_ChangesFilterViewAsync()
    {
        List<Sim> sims = new List<Sim>()
        {
            new Sim() {PhoneNumber = "A1", SimNumber = "1.1"},
            new Sim() {PhoneNumber = "B2", SimNumber = "2.2"},
            new Sim() {PhoneNumber = "C3", SimNumber = "3.3"}
        };

        SimsMainViewModel vm = ViewModelMockSetup(sims);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.SimItems);

        vm.FilterPhoneNumber = "B2";

        var actual = view.OfType<SimsItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(sims[1].PhoneNumber, actual[0].PhoneNumber);
    }

    [Fact]
    public async Task ChangingFilterSimNumber_ChangesFilterViewAsync()
    {
        List<Sim> sims = new List<Sim>()
        {
            new Sim() {PhoneNumber = "A1", SimNumber = "101"},
            new Sim() {PhoneNumber = "B2", SimNumber = "202"},
            new Sim() {PhoneNumber = "C3", SimNumber = "333"}
        };

        SimsMainViewModel vm = ViewModelMockSetup(sims);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.SimItems);

        vm.FilterSimNumber = "20";

        var actual = view.OfType<SimsItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(sims[1].SimNumber, actual[0].SimNumber);
    }

    [Fact]
    public async Task ChangingFilterStatus_ChangesFilterViewAsync()
    {
        List<Sim> sims = new List<Sim>()
        {
            new Sim() {PhoneNumber = "1", SimNumber = "1.1", Status = "B1"},
            new Sim() {PhoneNumber = "2", SimNumber = "2.2", Status = "B2"},
            new Sim() {PhoneNumber = "3", SimNumber = "3.3"}
        };

        SimsMainViewModel vm = ViewModelMockSetup(sims);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.SimItems);

        vm.FilterStatus = "B2";

        var actual = view.OfType<SimsItemViewModel>().ToArray();
        Assert.Single(actual);
        Assert.Equal(sims[1].Status, actual[0].Status);
    }

    private int index = 0;
    private SimsMainViewModel ViewModelMockSetup(List<Sim> sims)
    {
        AutoMocker mocker = new AutoMocker();

        Mock<ISimsRepository> repository = mocker.GetMock<ISimsRepository>();
        repository.Setup(r => r.GetSimsAsync()).ReturnsAsync(sims);
        Mock<ISimsItemViewModelFactory> factory = mocker.GetMock<ISimsItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<Sim>()))
                            .Returns(() => {
                                return new SimsItemViewModel(repository.Object, sims[index]);                                
                            }
                            )
                            .Callback(() => index++);

        SimsMainViewModel vm = mocker.CreateInstance<SimsMainViewModel>();
        return vm;
    }

}
