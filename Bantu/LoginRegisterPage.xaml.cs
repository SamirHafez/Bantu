using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Bantu.Azure;
using Bantu.ViewModel;

namespace Bantu
{
    public partial class LoginRegisterPage : PhoneApplicationPage
    {
        public LoginRegisterPage()
        {
            InitializeComponent();
        }

        public void Setup(object sender, EventArgs e)
        {
            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        public void Login(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;

            var username = tbUsername.Text;
            var password = pbPassword.Password;

            Context.Login(username, password, player =>
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Add("player", new PlayerVM(player));
                Dispatcher.BeginInvoke(delegate()
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }, () =>
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    pbPassword.Password = string.Empty;
                    MessageBox.Show("Login failed. Check your username and/or password and try again.");
                });
            });
        }

        public void Signup(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;

            var username = tbUsernameSign.Text;
            var password = pbPasswordSign.Password;

            Context.CreatePlayer(username, password, player =>
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Add("player", new PlayerVM(player));
                Dispatcher.BeginInvoke(delegate()
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }, () =>
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("Signup failed. The selected username is unavailable.");
                });
            });
        }
    }
}