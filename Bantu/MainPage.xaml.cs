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
using Bantu.Model;
using Microsoft.WindowsAzure.Samples.Phone.Storage;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace Bantu
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static Player Player { get; set; }
        public static ObservableCollection<Game> ActiveGames { get; set; }
        private ObservableCollection<Game> OpenGames { get; set; }

        private int _currentGame;

        private IsolatedStorageSettings _settings;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //ModelHelpers.Reset();
            ModelHelpers.CreateGame("lalala", null, null);

            _settings = IsolatedStorageSettings.ApplicationSettings;
            if (_settings.Contains("player"))
                Player = _settings["player"] as Player;

            if (_settings.Contains("activeGames"))
                ActiveGames = _settings["activeGames"] as ObservableCollection<Game>;
            else
                ActiveGames = new ObservableCollection<Game>();

            OpenGames = new ObservableCollection<Game>();

            lbQuickPlay.ItemsSource = OpenGames;
            lbActiveGames.ItemsSource = ActiveGames;
            _currentGame = 0;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            _settings["activeGames"] = ActiveGames.ToList();
            base.OnBackKeyPress(e);
        }

        public void Setup(object sender, EventArgs e) 
        {
            if (Player == null)
                NavigationService.Navigate(new Uri("/LoginRegisterPage.xaml", UriKind.Relative));

            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        public void SwitchPanoramaItem(object sender, EventArgs e)
        {
            Panorama panorama = (Panorama)sender;
            PanoramaItem panoramaItem = (PanoramaItem)(panorama.SelectedItem);
            if (panoramaItem.Name.Equals("qp"))
                LoadOpenGames(this, null);
        }

        public void LoadOpenGames(object sender, EventArgs e) 
        {
            if (sender is PanoramaItem)
                return;

            SystemTray.ProgressIndicator.Text = "Finding available games";
            SystemTray.ProgressIndicator.IsVisible = true;

            ModelHelpers.OpenGames(og => Dispatcher.BeginInvoke(delegate() 
            {
                OpenGames.Clear();
                foreach(Game game in og) OpenGames.Add(game);
                SystemTray.ProgressIndicator.IsVisible = false;
            }), () => Dispatcher.BeginInvoke(delegate() 
            {
                MessageBox.Show("Error loading games. Try again.");
                SystemTray.ProgressIndicator.IsVisible = false;
            }));
        }

        public void AcceptChallenge(object sender, GestureEventArgs e) 
        {
            SystemTray.ProgressIndicator.Text = "Accepting challenge";
            SystemTray.ProgressIndicator.IsVisible = true;

            var game = (Game)((FrameworkElement)e.OriginalSource).DataContext;
            ModelHelpers.JoinGame(Player.Name, game, g => Dispatcher.BeginInvoke(delegate() 
            {
                ActiveGames.Add(g);
                _currentGame = ActiveGames.IndexOf(g);
                SystemTray.ProgressIndicator.IsVisible = false;
                NavigationService.Navigate(new Uri("/GamePage.xaml?game=" + _currentGame, UriKind.Relative));
            }), g => Dispatcher.BeginInvoke(delegate() 
            {
                MessageBox.Show("This challenge has already been accepted by someone else. Try another one.");
                OpenGames.Remove(g);
                SystemTray.ProgressIndicator.IsVisible = false;
            }));
        }
    }
}