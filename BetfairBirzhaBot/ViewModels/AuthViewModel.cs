using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        public IAsyncCommand LoginCommand { get; set; }
        public string Password { get; set; }

        public AuthViewModel()
        {
            LoginCommand = new AsyncCommand(Login);
        }

        private async Task Login()
        {
            if (string.IsNullOrEmpty(Password))
                return;

            if (Password != "venice-bf-3c771f06")
                return;

            var currentWindow = App.ServiceProvider.GetService<AuthWindow>();
            var mainWindow = App.ServiceProvider.GetService<MainWindow>();
            var mainWindowVM = App.ServiceProvider.GetService<MainWindowViewModel>();

            mainWindow.DataContext = mainWindowVM;

            mainWindow.Show();
            currentWindow.Close();
        }
    }
}
