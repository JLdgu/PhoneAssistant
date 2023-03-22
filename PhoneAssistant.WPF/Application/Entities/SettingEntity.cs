﻿namespace PhoneAssistant.WPF.Application.Entities;

public class SettingEntity
{
    public int Id { get; set; }

    public string MinimumVersion { get; set; }

    public SettingEntity(int id, string minimumVersion)
    {
        Id = id;
        MinimumVersion = minimumVersion;
    }
}
