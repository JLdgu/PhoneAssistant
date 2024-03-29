﻿using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsItemViewModelFactory : ISimsItemViewModelFactory
{
    private readonly ISimsRepository _simsRepository;

    public SimsItemViewModelFactory(ISimsRepository simsRepository)
    {
        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
    }

    public SimsItemViewModel Create(Sim sim) => new SimsItemViewModel(_simsRepository,sim);    
}
