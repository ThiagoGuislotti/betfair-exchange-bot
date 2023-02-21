using BetfairBirzhaBot.Core;
using BetfairBirzhaBot.Core.Telegram;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Services;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using BetfairBirzhaBot.ViewModels;
using BetfairBirzhaBot.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace BetfairBirzhaBot
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider;
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<SygnalProcessService>();
            services.AddSingleton<BotParserService>();
            services.AddSingleton<BotBettingService>();
            services.AddSingleton<LogService>();

            services.AddSingleton<BetfairBirzhaBotCore>();
            services.AddSingleton<FilterFactory>();
            services.AddSingleton<TelegramSygnalBot>();

            //можно добавить навигатор для связывания ViewModel и View
            //в следущей жизни
            
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AuthWindow>();

            services.AddSingleton<AuthViewModel>();
            services.AddSingleton<BotControlPanelViewModel>();
            services.AddSingleton<StrategyManagerViewModel>();
            services.AddSingleton<GameInplayViewerViewModel>();
            services.AddSingleton<LeaguesBlackListViewModel>();
            services.AddSingleton<SygnalsStoreViewModel>();
            services.AddSingleton<TelegramSettingsViewModel>();
            services.AddSingleton<MainWindowViewModel>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //var passwordView = ServiceProvider.GetRequiredService<AuthWindow>();
            //var passwordViewModel = ServiceProvider.GetRequiredService<AuthViewModel>();
            //passwordView.DataContext = passwordViewModel;
            //passwordView.Show();
            var view = ServiceProvider.GetRequiredService<MainWindow>();
            var viewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();

            view.DataContext = viewModel;

            view.Show();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            using var sw = new StreamWriter(Environment.CurrentDirectory + @"\Crash.txt", false, System.Text.Encoding.UTF8);
            sw.WriteLine(ex.Message);
            sw.WriteLine("================================");
            sw.WriteLine(ex.StackTrace);
            sw.WriteLine("================================");
            sw.WriteLine(ex.InnerException?.Message);
            sw.WriteLine("================================");
            sw.WriteLine(ex.InnerException?.StackTrace);
        }
    }

}
