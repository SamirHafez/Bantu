using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Bantu.ViewModel;
using Bantu.Azure;

namespace Bantu
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static PlayerVM Player { get; set; }
        public static ObservableCollection<GameVM> Games { get; set; }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("player"))
                Player = settings["player"] as PlayerVM;

            if (settings.Contains("games"))
                Games = new ObservableCollection<GameVM>(settings["games"] as IEnumerable<GameVM>);
            else
                Games = new ObservableCollection<GameVM>();
        }

        public void Initialize(Object sender, EventArgs e)
        {
            if (Player == null)
            {
                NavigationService.Navigate(new Uri("/LoginRegisterPage.xaml", UriKind.Relative));
                return;
            }

            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        public void GoToGame(Object sender, GestureEventArgs e)
        {
            var game = (GameVM)((FrameworkElement)e.OriginalSource).DataContext;

            if (game.Client == null || (game.HostTurn && game.Host.Name != Player.Name) || (!game.HostTurn && game.Client.Name != Player.Name))
                return;

            var index = Games.IndexOf(game);
            NavigationService.Navigate(new Uri("/BantumiGamePage.xaml?game=" + index, UriKind.Relative));
        }

        public void CreateGame(Object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            Context.CreateGame(Player.Name, game =>
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    Games.Add(new GameVM(game));

                    if (settings.Contains("games"))
                        settings["games"] = Games.ToArray();
                    else
                        settings.Add("games", Games.ToArray());
                    SystemTray.ProgressIndicator.IsVisible = false;
                });
            }, () =>
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("Failed to create game. Please try again.");
                });
            });
        }

        private void ResetState()
        {
            Context.Reset();

            var settings = IsolatedStorageSettings.ApplicationSettings;

            settings.Remove("player");
            settings.Remove("games");
        }
    }
}