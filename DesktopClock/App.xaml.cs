using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DesktopClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // DIセットアップ
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            // メインウィンドウの表示
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<ViewModels.MainPageViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();

            base.OnExit(e);
        }
    }

}
