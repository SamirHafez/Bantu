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
using System.Windows.Controls;

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

            //ResetState();

            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("player"))
            {
                Player = settings["player"] as PlayerVM;
                userPi.Header = Player.Name;
            }

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
            OpenGame(game);
        }

        private void OpenGame(GameVM game)
        {
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

                    settings["games"] = Games.ToArray();
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

        public void Refresh(Object sender, EventArgs e)
        {
            foreach (var gameVM in Games)
            {
                SystemTray.ProgressIndicator.IsVisible = true;
                Context.GetGame(gameVM.Id, game =>
                {
                    Dispatcher.BeginInvoke(delegate()
                    {
                        gameVM.Update(game);
                        SystemTray.ProgressIndicator.IsVisible = false;
                    });
                }, () =>
                {
                    Dispatcher.BeginInvoke(delegate()
                    {
                        SystemTray.ProgressIndicator.IsVisible = false;
                        //MessageBox.Show("Failed to update game. Please try again.");
                    });
                }, gameVM.LastUpdate);
            }
        }

        public void JoinRandom(Object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            Context.OpenGames(Player.Name, games =>
            {
                Context.JoinGame(Player.Name, games.First(), game =>
                {
                    Dispatcher.BeginInvoke(delegate()
                    {
                        var settings = IsolatedStorageSettings.ApplicationSettings;
                        var gameVm = new GameVM(game);
                        Games.Add(gameVm);

                        settings["games"] = Games.ToArray();
                        SystemTray.ProgressIndicator.IsVisible = false;
                        OpenGame(gameVm);
                    });
                }, game =>
                {
                    Dispatcher.BeginInvoke(delegate()
                    {
                        SystemTray.ProgressIndicator.IsVisible = false;
                        MessageBox.Show("Failed to join a challenge. Please try again.");
                    });
                });
            }, () =>
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("Failed to find an open challenge. Please try again.");
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

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Panorama panorama = (Panorama)sender;
            PanoramaItem panoramaItem = (PanoramaItem)(panorama.SelectedItem);
            if (panoramaItem.Name.Equals("agPi"))
                Refresh(this, null);
        }
    }
}