using System.Diagnostics.CodeAnalysis;

namespace PhoneAssistant.WPF.Application.Entities;

public class UpdateHistoryPhone
{
    public UpdateHistoryPhone()
    {
        
    }

    [SetsRequiredMembers]
    public UpdateHistoryPhone(Phone phone, UpdateTypes updateType)
    {
        UpdateType = updateType;
        Imei = phone.Imei;
        PhoneNumber = phone.PhoneNumber;
        SimNumber = phone.SimNumber;
        FormerUser = phone.FormerUser;
        Condition = phone.Condition;
        Status = phone.Status;
        OEM = phone.OEM;
        Model = phone.Model;
        SR = phone.SR;
        AssetTag = phone.AssetTag;
        NewUser = phone.NewUser;
        Notes = phone.Notes;
        DespatchDetails = phone.DespatchDetails;
        LastUpdate = phone.LastUpdate;
    }

    public int Id { get; set; }
    public required UpdateTypes UpdateType { get; set; }
    public required string Imei { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SimNumber { get; set; }
    public string? FormerUser { get; set; }
    public required string Condition { get; set; }
    public required string Status { get; set; }
    public required OEMs OEM { get; set; }
    public required string Model { get; set; }
    public int? SR { get; set; }
    public string? AssetTag { get; set; }
    public string? NewUser { get; set; }
    public string? Notes { get; set; }
    public string? DespatchDetails { get; set; }
    public required string LastUpdate { get; set; }
}

public enum UpdateTypes
{
    DELETE,
    UPDATE
}
