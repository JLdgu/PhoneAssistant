﻿using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public interface ISimsItemViewModelFactory
{
    SimsItemViewModel Create(Sim sim);
}
