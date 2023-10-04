﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.MainWindow;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Users;

namespace PhoneAssistant.WPF.Application;

public static class ApplicationServicesExtensions
{
    public static IHostBuilder ConfigureApplicationServices(this IHostBuilder host)
    {
        host.ConfigureServices((context, services) =>
        {
            // Application
            UserSettings settings = new();
            string database = settings.Database;
            string connectionString = $@"DataSource={database};";
            services.AddDbContext<v1PhoneAssistantDbContext>(
                            options => options.UseSqlite(connectionString),
                            ServiceLifetime.Singleton);
            services.AddSingleton<IUserSettings, UserSettings>();

            // Features
            services.AddTransient<IDashboardMainViewModel, DashboardMainViewModel>();

            services.AddTransient<IPhonesMainViewModel, PhonesMainViewModel>();
            services.AddTransient<IPhonesItemViewModelFactory, PhonesItemViewModelFactory>();
            services.AddSingleton<IPhonesRepository, PhonesRepository>();
            services.AddTransient<IPrintEnvelope, PrintEnvelope>();

            services.AddTransient<ISettingsMainViewModel, SettingsMainViewModel>();
            services.AddTransient<IThemeWrapper, ThemeWrapper>();

            services.AddTransient<IUsersMainViewModel, UsersMainViewModel>();
            services.AddTransient<IUsersItemViewModelFactory, UsersItemViewModelFactory>();
            
            services.AddTransient<ISimsMainViewModel, SimsMainViewModel>();
            services.AddSingleton<ISimsRepository, SimsRepository>();
            services.AddTransient<Func<v1Sim, SimsItemViewModel>>(serviceProvider =>
            {
                return (v1Sim sim) => new SimsItemViewModel(sim,
                                      serviceProvider.GetRequiredService<ISimsRepository>());
            });
            
            services.AddTransient<MainWindowViewModel>();
            services.AddSingleton(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));
        });
        return host;
    }
}