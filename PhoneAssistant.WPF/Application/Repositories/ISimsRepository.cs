﻿using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface ISimsRepository
{
    Task CreateAsync(Sim sim);
    //Task<string?> DeleteSIMAsync(string phoneNumber);
    //Task<string?> GetSIMNumberAsync(string phoneNumber);
    Task<IEnumerable<Sim>> GetSimsAsync();
}