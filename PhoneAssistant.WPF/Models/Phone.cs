using System.Reflection;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Models;

public sealed class Phone
{
    public int Id { get; init; }

    public string IMEI { get; init; }

    public string? FormerUser { get; init; }

    public bool Wiped { get; init; }

    public string Status { get; init; }

    public string OEM { get; init; }

    public string? AssetTag { get; init; }

    public string? Note { get; init; }

    public Phone(int id, string iMEI, string? formerUser, bool wiped, string status, string oEM, string? assetTag, string? note)
    {
        Id = id;
        IMEI = iMEI;
        FormerUser = formerUser;
        Wiped = wiped;
        Status = status;
        OEM = oEM;
        AssetTag = assetTag;
        Note = note;
    }

    public static Phone ToPhone(PhoneEntity entity)
    {
        return new Phone(
            entity.Id,
            entity.IMEI,
            entity.FormerUser,
            entity.Wiped,
            entity.Status,
            entity.OEM,
            entity.AssetTag,
            entity.Note);
    }

    public static implicit operator PhoneEntity(Phone phone)
    {
        return new PhoneEntity
        {
            Id = phone.Id,
            IMEI = phone.IMEI,
            FormerUser = phone.FormerUser,
            Wiped = phone.Wiped,
            Status = phone.Status,
            OEM = phone.OEM,
            AssetTag = phone.AssetTag,
            Note = phone.Note
        };
    }
}