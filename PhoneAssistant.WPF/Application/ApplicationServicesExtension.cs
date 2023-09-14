using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.MainWindow;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Users;

namespace PhoneAssistant.WPF.Application;

public static class ApplicationServicesExtensions
{
    public static IHostBuilder ConfigureApplicationServices(this IHostBuilder host)
    {
        host.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IUserSettings, UserSettings>();
            services.AddTransient<Func<User, UsersItemViewModel>>(serviceProvider =>
            {
                return (Features.Users.User user)  => new UsersItemViewModel(user);
            });

            services.AddTransient<IDashboardMainViewModel, DashboardMainViewModel>();
            services.AddTransient<IPrintEnvelope, PrintEnvelope>();
            services.AddTransient<IPhonesMainViewModel, PhonesMainViewModel>();
            services.AddTransient<ISettingsMainViewModel, SettingsMainViewModel>();
            services.AddTransient<IUsersMainViewModel, UsersMainViewModel>();

            UserSettings settings = new();
            string database = settings.Database;
            string connectionString = $@"DataSource={database};";
            services.AddDbContext<v1PhoneAssistantDbContext>(
                            options => options.UseSqlite(connectionString),
                            ServiceLifetime.Singleton);

            services.AddTransient<MainWindowViewModel>();
            services.AddSingleton(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));
        });
        return host;
    }
}