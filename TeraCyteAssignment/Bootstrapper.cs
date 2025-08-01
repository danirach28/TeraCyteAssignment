using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TeraCyteAssignment.Views;
using Windows.Services.Maps;
using Windows.UI.ViewManagement;

namespace TeraCyteAssignment;

public static class Bootstrapper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
    }
}
