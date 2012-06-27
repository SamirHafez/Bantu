using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Bantu.Helpers;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.ComponentModel;

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

            ModelHelpers.Login(username, password, player =>
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Add("player", player);
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

            ModelHelpers.CreatePlayer(username, password, player =>
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Add("player", player);
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
                    MessageBox.Show("Signup failed. That username is unavailable.");
                });
            });
        }
    }
}