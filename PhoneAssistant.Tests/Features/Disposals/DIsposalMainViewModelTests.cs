using Moq.AutoMock;

using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class DisposalMainViewModelTests()
{
    [Fact]
    public void Receive_ShouldSetMaxProgress_WhenMessageTypeMaxProgress()
    {
        AutoMocker _mocker = new();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.MaxProgress, "", 257));

        Assert.Equal(257, sut.MaxProgress);
        Assert.Single(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldSetProgress_WhenMessageTypeProgress()
    {
        AutoMocker _mocker = new();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.Progress, "", 157));

        Assert.Equal(157, sut.Progress);
        Assert.Empty(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldUpdateLogItems_WhenMessageTypeDefault()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.Default, "Some message"));

        Assert.Single( sut.LogItems);
    }
}
