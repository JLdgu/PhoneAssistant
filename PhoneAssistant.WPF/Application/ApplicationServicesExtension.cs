using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.MainWindow;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
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
            //services.AddTransient<IPhonesRepository, PhonesRepository>();
            //services.AddTransient<ISimsRepository, SimsRepository>();
            //services.AddTransient<IServiceRequestsRepository, ServiceRequestsRepository>();
            services.AddSingleton<IUserSettings, UserSettings>();
            //services.AddTransient<IStateRepository, StateRepository>();

            services.AddTransient<IDashboardMainViewModel, DashboardMainViewModel>();
            services.AddTransient<IPrintEnvelope, PrintEnvelope>();
            services.AddTransient<IPhonesMainViewModel, PhonesMainViewModel>();
            //services.AddTransient<ISimsMainViewModel, SimsMainViewModel>();
            //services.AddTransient<IServiceRequestsMainViewModel, ServiceRequestsMainViewModel>();
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