﻿namespace PhoneAssistant.WPF.Application.Entities;

public class Phone
{
    public int Id { get; set; }

    public string Imei { get; set; }

    public string? FormerUser { get; set; }

    public bool Wiped { get; set; }

    public string Status { get; set; }

    public string OEM { get; set; }

    public string? AssetTag { get; set; }

    public string? Note { get; set; }

    // Navigation properties
    //public virtual LinkDTO? Link { get; set; }
}