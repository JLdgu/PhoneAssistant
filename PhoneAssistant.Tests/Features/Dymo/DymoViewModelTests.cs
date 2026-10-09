using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Dymo;

namespace PhoneAssistant.Tests.Features.Dymo;

public sealed class DymoViewModelTests
{
    [Test]
    public async Task SelectedLocationChanged_UpdatesIncludeDateFromCollection()
    {
        AutoMocker mocker = new();
        mocker.GetMock<ILocationsRepository>()
            .Setup(repository => repository.GetAllLocationsAsync())
            .ReturnsAsync(Array.Empty<Location>());

        DymoViewModel viewModel = mocker.CreateInstance<DymoViewModel>();
        await viewModel.LoadAsync();

        viewModel.SelectedLocation = new Location
        {
            Name = "Collection",
            Address = "Collection address",
            Collection = true
        };

        await Assert.That(viewModel.IncludeDate).IsTrue();

        viewModel.SelectedLocation = new Location
        {
            Name = "Delivery",
            Address = "Delivery address",
            Collection = false
        };

        await Assert.That(viewModel.IncludeDate).IsFalse();
    }
}
