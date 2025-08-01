using System.Configuration;
using System.Data;
using System.Windows;
using TeraCyteAssignment;
using Microsoft.Extensions.DependencyInjection;
using TeraCyteAssignment.Views;

namespace TeraCyteAssignment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();
            Bootstrapper.ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}