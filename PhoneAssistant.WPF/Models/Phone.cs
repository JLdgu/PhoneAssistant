﻿using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Models;

public sealed class Phone
{
    public int Id { get; set; }

    public string Imei { get; set; }

    public string? FormerUser { get; set; }

    public bool Wiped { get; set; }

    public string Status { get; set; }

    public string OEM { get; set; }

    public string? AssetTag { get; set; }

    public string? Note { get; set; }

    public Phone(int id, string iMEI, string? formerUser, bool wiped, string status, string oEM, string? assetTag, string? note)
    {
        Id = id;
        Imei = iMEI;
        FormerUser = formerUser;
        Wiped = wiped;
        Status = status;
        OEM = oEM;
        AssetTag = assetTag;
        Note = note;
    }
    
    public static Phone ToPhone(PhoneEntity entity) => 
        new(id: entity.Id,
            iMEI: entity.Imei,
            formerUser: entity.FormerUser,
            wiped: entity.Wiped,
            status: entity.Status,
            oEM: entity.OEM,
            assetTag: entity.AssetTag,
            note: entity.Note);

    public static implicit operator PhoneEntity(Phone phone)
    {
        return new PhoneEntity
        {
            Id = phone.Id,
            Imei = phone.Imei,
            FormerUser = phone.FormerUser,
            Wiped = phone.Wiped,
            Status = phone.Status,
            OEM = phone.OEM,
            AssetTag = phone.AssetTag,
            Note = phone.Note
        };
    }
}