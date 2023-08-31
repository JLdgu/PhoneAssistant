﻿using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public interface IPhonesRepository
{
    Task<IEnumerable<v1Phone>> GetPhonesAsync();
    //Task<IEnumerable<Phone>> SearchAsync(string search);
    //Task UpdateAsync(Phone phoneToUpdate);
}