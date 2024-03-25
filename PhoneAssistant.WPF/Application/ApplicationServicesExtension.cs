using CommunityToolkit.Mvvm.Messaging;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.BaseReport;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Disposals;
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
            services.AddDbContext<PhoneAssistantDbContext>(
                            options => options.UseSqlite(connectionString),
                            ServiceLifetime.Singleton);
            services.AddSingleton<IUserSettings, UserSettings>();

            services.AddSingleton<WeakReferenceMessenger>();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider => provider.GetRequiredService<WeakReferenceMessenger>());

            // Features
            services.AddSingleton<BaseReportRepository>();
            services.AddSingleton<IBaseReportMainViewModel, BaseReportMainViewModel>();

            services.AddSingleton<IDashboardMainViewModel, DashboardMainViewModel>();

            services.AddSingleton<DisposalsRepository>();
            services.AddSingleton<IDisposalsMainViewModel, DisposalsMainViewModel>();

            services.AddSingleton<ILocationsRepository, LocationsRepository>();
            services.AddSingleton<IPhonesRepository, PhonesRepository>();
            services.AddTransient<IPhonesItemViewModelFactory, PhonesItemViewModelFactory>();
            services.AddSingleton<EmailViewModel>();
            services.AddTransient<PhonesItemViewModel>();
            services.AddTransient<IPrintEnvelope, PrintEnvelope>();
            services.AddTransient<IPrintDymoLabel, PrintDymoLabel>();
            services.AddTransient<IPhonesMainViewModel, PhonesMainViewModel>();

            services.AddSingleton<ISettingsMainViewModel, SettingsMainViewModel>();
            services.AddTransient<IThemeWrapper, ThemeWrapper>();

            services.AddTransient<IUsersMainViewModel, UsersMainViewModel>();
            services.AddTransient<IUsersItemViewModelFactory, UsersItemViewModelFactory>();
            
            services.AddSingleton<ISimsRepository, SimsRepository>();
            services.AddSingleton<ISimsMainViewModel, SimsMainViewModel>();
            services.AddTransient<ISimsItemViewModelFactory, SimsItemViewModelFactory>();
            services.AddTransient<SimsItemViewModel>();
            
            services.AddTransient<MainWindowViewModel>();
            services.AddSingleton(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));
        });
        return host;
    }
}