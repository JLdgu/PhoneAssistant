﻿using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Application;
public sealed class StateRepository
{
    public async Task<IEnumerable<State>?> AllAsync()
    {
        await Task.CompletedTask;
        return AllStates();
    }

    private static IEnumerable<State> AllStates()
    {
        return new List<State>()
        {
            new State () {Status = "In Stock"},
            new State () {Status = "In Repair"},
            new State () {Status = "Production"},
            new State () {Status = "Decomissioned"},
            new State () {Status = "Disposed"}
        };
    }

}