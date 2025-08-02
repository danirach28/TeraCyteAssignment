using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TeraCyteAssignment.Configuration;
using TeraCyteAssignment.Services.Interface;
using TeraCyteAssignment.Services;
using TeraCyteAssignment.ViewModels;
using TeraCyteAssignment.Views;
using System.Net.Http;


namespace TeraCyteAssignment
{
    public static class Bootstrapper
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

            var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
            var credentials = configuration.GetSection("Credentials").Get<Credentials>() ?? new Credentials();

            services.AddSingleton(apiSettings);
            services.AddSingleton(credentials);

            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(apiSettings.BaseUrl)
            });

            services.AddSingleton<IAuthService, AuthService>();

            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IDataPollingService, DataPollingService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}

