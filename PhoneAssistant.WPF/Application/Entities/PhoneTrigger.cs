using EntityFrameworkCore.Triggered;

namespace PhoneAssistant.WPF.Application.Entities;

public sealed class PhoneTrigger : IBeforeSaveTrigger<Phone>
{
    public Task BeforeSave(ITriggerContext<Phone> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType != ChangeType.Deleted)
            context.Entity.LastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");          

        return Task.CompletedTask;
    }
}
