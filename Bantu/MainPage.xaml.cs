﻿using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Bantu.ViewModel;
using Bantu.TableStorage;
using System.Windows.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Bantu.Notification;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Bantu
{
	public partial class MainPage : PhoneApplicationPage
	{
		public static PlayerVM Player { get; set; }
		public ObservableCollection<GameVM> Games { get; set; }
		public static bool NewPlayer { get; set; }
		public SettingsVM Settings { get; set; }

		public MainPage()
		{
			InitializeComponent();
			DataContext = this;

			Games = new ObservableCollection<GameVM>();
			Player = new PlayerVM();
			Settings = new SettingsVM();

			Manager.GameEventToast += ReceivedToast;
		}

		public void Initialize(Object sender, EventArgs e)
		{
            if (!NetworkInterface.GetIsNetworkAvailable())
			{
				MessageBox.Show("Bantu requires an active internet connection. Exiting.");
				NavigationService.GoBack();
			}

			var settings = IsolatedStorageSettings.ApplicationSettings;

			if (settings.Contains("player"))
			{
                var player = (PlayerVM)settings["player"];
				Player.Name = player.Name;
				Player.Score = player.Score;
				Player.Identifier = player.Identifier;
				userPi.Header = Player.Name;
			}
			else 
			{
				NavigationService.Navigate(new Uri("/Pages/LoginPage.xaml", UriKind.Relative));
				return;
			}

			while (NavigationService.BackStack.Any())
				NavigationService.RemoveBackEntry();

			if (settings.Contains("settings"))
				Settings.Notifications = ((SettingsVM) settings["settings"]).Notifications;

			if (Settings.Notifications)
				Manager.EnableNotifications(Player.Name);

            if (NewPlayer)
            {
				NewPlayer = false;
                var result = MessageBox.Show("Since you are a new player would you like to learn how to play?", "NEW PLAYER", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                    NavigationService.Navigate(new Uri("/Pages/Help.xaml", UriKind.Relative));
            }
            else
            {
                RefreshPlayer();
                RefreshGames();
            }
		}

		private void ReceivedToast(string gameId)
		{
			Context.GetGame(gameId, game =>
				Dispatcher.BeginInvoke(() =>
				{
					var gameVm = Games.First(g => g.Id == gameId);
					gameVm.Update(game);

					var settings = IsolatedStorageSettings.ApplicationSettings;
					settings["games"] = Games.ToArray();
				})
			, () =>
			{
			});
		}

		public void GoToGame(Object sender, GestureEventArgs gestureEventArgs)
		{
			var game = Games.First(g => g.Id == (string)(((Button)sender).Tag));
			OpenGame(game);
		}

		public void HelpPage(Object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Pages/Help.xaml", UriKind.Relative));
		}

		public void AboutPage(Object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Pages/About.xaml", UriKind.Relative));
		}

		public void SettingsPage(Object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
		}

		private void OpenGame(GameVM game)
		{
			NavigationService.Navigate(new Uri("/Pages/BantumiGamePage.xaml?game=" + game.Id, UriKind.Relative));
		}

		public void CreateGame(Object sender, EventArgs e)
		{
			if (Games.Count >= 10)
				MessageBox.Show("You are not allowed to play in more than ten games simultaneously. Please finish some games before starting new ones.");

			SystemTray.ProgressIndicator.IsVisible = true;
			Context.CreateGame(Player.Name, game =>
				Dispatcher.BeginInvoke(() =>
				{
					var settings = IsolatedStorageSettings.ApplicationSettings;
					Games.Add(new GameVM(game));

					settings["games"] = Games.ToArray();
					SystemTray.ProgressIndicator.IsVisible = false;
				})
			, () =>
				Dispatcher.BeginInvoke(() =>
				{
					SystemTray.ProgressIndicator.IsVisible = false;
					MessageBox.Show("Failed to create game. Please try again.");
				})
			);
		}

        public void Refresh(object sender, EventArgs args) 
        {
            RefreshPlayer();
            RefreshGames();
        }

        private void RefreshPlayer() 
        {
            SystemTray.ProgressIndicator.IsVisible = true;

            Context.GetPlayerByName(Player.Name, player =>
                Dispatcher.BeginInvoke(() =>
                {
                    Player.Score = player.Score;
                    SystemTray.ProgressIndicator.IsVisible = false;
                })
            , () =>
            {
            });
        }

		private void RefreshGames()
		{
			SystemTray.ProgressIndicator.IsVisible = true;

			Context.PlayerGames(Player.Name, games =>
				Dispatcher.BeginInvoke(() =>
				{
					Games.Clear();
					foreach (var game in games.Select(g => new GameVM(g)))
						Games.Add(game);

                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    settings["games"] = Games.ToArray();

                    SystemTray.ProgressIndicator.IsVisible = false;
				})
			, () =>
			{
			});
		}

		public void JoinRandom(Object sender, EventArgs e)
		{
			if (Games.Count >= 10)
				MessageBox.Show("You are not allowed to play in more than ten games simultaneously. Please finish some games before starting new ones.");

			SystemTray.ProgressIndicator.IsVisible = true;
			Context.JoinGame(Player.Name, game =>
				Dispatcher.BeginInvoke(() =>
				{
					var settings = IsolatedStorageSettings.ApplicationSettings;
					var gameVm = new GameVM(game);
					Games.Add(gameVm);

					settings["games"] = Games.ToArray();
					SystemTray.ProgressIndicator.IsVisible = false;
					OpenGame(gameVm);
				})
			, () =>
				Dispatcher.BeginInvoke(() =>
				{
					SystemTray.ProgressIndicator.IsVisible = false;
					MessageBox.Show("Failed to join a challenge. Please try again.");
				})
			);
		}
	}
}