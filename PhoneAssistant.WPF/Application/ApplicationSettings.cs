﻿namespace PhoneAssistant.WPF.Application;

public static class ApplicationSettings
{
    public const string ConditionNew = "New";
    public const string ConditionRepurposed = "Repurposed";

    public static readonly List<string> Conditions = [ConditionNew, ConditionRepurposed];

    public const string StatusDecommissioned = "Decommissioned";
    public const string StatusDisposed = "Disposed";
    public const string StatusInRepair = "In Repair";
    public const string StatusInStock = "In Stock";
    public const string StatusMisplaced = "Misplaced";
    public const string StatusProduction = "Production";

    public static readonly List<string> Statuses = [StatusProduction, StatusInStock, StatusInRepair, StatusDecommissioned, StatusDisposed, StatusMisplaced];

}
