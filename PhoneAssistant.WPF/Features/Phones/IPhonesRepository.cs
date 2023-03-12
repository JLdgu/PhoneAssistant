﻿using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Phones;
public interface IPhonesRepository
{
    Task<IEnumerable<Phone>> GetPhonesAsync();
    Task<IEnumerable<Phone>> SearchAsync(string search);
    Task UpdateAsync(Phone phoneToUpdate);
}