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

        public App()
        {
            // DIセットアップ
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                await _serviceProvider.GetRequiredService<SettingManager>().LoadSettingsAsync();

                await _serviceProvider.GetRequiredService<Logger>().CollectAsync();

                // メインウィンドウの表示
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetRequiredService<Logger>().LogAsync($"[{ex.GetType()}] {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<ViewModels.MainPageViewModel>();

            services.AddSingleton<General>();
            services.AddSingleton<Logger>();
            services.AddSingleton<SettingManager>();
            services.AddSingleton<NotifyIcon>();
            services.AddSingleton<StartupRegister>();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                await _serviceProvider.GetRequiredService<SettingManager>().SaveSettingsAsync();
                await _serviceProvider.DisposeAsync();

                base.OnExit(e);
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetRequiredService<Logger>().LogAsync($"[{ex.GetType()}] {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }

}
