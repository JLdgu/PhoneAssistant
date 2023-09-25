using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddSingleton<IPhonesRepository, PhonesRepository>();
            services.AddTransient<IPrintEnvelope, PrintEnvelope>();

            services.AddTransient<ISettingsMainViewModel, SettingsMainViewModel>();
            services.AddTransient<IThemeWrapper, ThemeWrapper>();

            services.AddTransient<IUsersMainViewModel, UsersMainViewModel>();
            services.AddTransient<Func<User, UsersItemViewModel>>(serviceProvider =>
            {
                return (Features.Users.User user)  => new UsersItemViewModel(user);
            });

            services.AddTransient<MainWindowViewModel>();
            services.AddSingleton(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));
        });
        return host;
    }
}