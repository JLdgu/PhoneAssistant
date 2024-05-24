using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class TrackProgressTests
{
    [Theory]
    [InlineData(71, 111)]
    [InlineData(173, 222)]
    [InlineData(72, 1000)]
    [InlineData(72, 2000)]
    public void Milestone_ShouldbeFalse_WhenMilestoneConditionsNotMet(int value, int maximum)
    {
        var sut = new TrackProgress(maximum);
        Assert.False(sut.Milestone(value));
    }

    [Theory]
    [InlineData(4, 10)]
    [InlineData(10, 10)]
    [InlineData(20, 50)]
    [InlineData(61, 100)]
    [InlineData(72, 100)]
    [InlineData(80, 100)]
    [InlineData(600, 2000)]
    [InlineData(250, 2540)]
    public void Milestone_ShouldbeTrue_WhenMilestoneConditionsMet(int value, int maximum)
    {
        var sut = new TrackProgress(maximum);
        Assert.True(sut.Milestone(value));        
    }
}
