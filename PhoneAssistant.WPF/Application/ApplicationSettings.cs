using System.Collections.Immutable;

namespace PhoneAssistant.WPF.Application;
public static class ApplicationSettings
{
    public static readonly List<string> Conditions = new() { "New", "Repurposed" };

    public static readonly List<string> Statuses = new() { "Production", "In Stock", "In Repair", "Decommissioned", "Disposed", "Misplaced" };
}
