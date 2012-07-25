﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Bantu.ViewModel;
using Bantu.TableStorage;
using System.Windows.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Bantu.Notification;

namespace Bantu
{
	public partial class MainPage : PhoneApplicationPage
	{
		public static PlayerVM Player { get; set; }
		public static ObservableCollection<GameVM> Games { get; set; }
		public static bool NewPlayer { get; set; }

		public MainPage()
		{
			InitializeComponent();
			DataContext = this;

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
			if (!InternetConectivity())
			{
				MessageBox.Show("Bantu requires an active internet connection. Exiting.");
				NavigationService.GoBack();
			}

			if (Player == null)
			{
				NavigationService.Navigate(new Uri("/LoginRegisterPage.xaml", UriKind.Relative));
				return;
			}

			Manager.EnableNotifications(Player.Name);

			while (NavigationService.BackStack.Any())
				NavigationService.RemoveBackEntry();

			RemoveWonGames();

			if (NewPlayer)
			{
				NewPlayer = false;
				var result = MessageBox.Show("Since you are a new player would you like to learn how to play?", "NEW PLAYER", MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.OK)
					NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
			}
		}

		private static void RemoveWonGames()
		{
			var removables = new List<GameVM>();
			removables.AddRange(Games.Where(g => g.State == GameState.Finished));
			foreach (var removable in removables)
			{
				Player.Score += removable.Winner.Score;
				Games.Remove(removable);
			}
		}

		public void GoToGame(Object sender, EventArgs e)
		{
			var game = Games.First(g => g.Id == (string)(((Button)sender).Tag));
			OpenGame(game);
		}

		public void HelpPage(Object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
		}

		public void AboutPage(Object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
		}

		private bool InternetConectivity()
		{
			return NetworkInterface.GetIsNetworkAvailable();
		}

		private void OpenGame(GameVM game)
		{
			if (game.Client == null || !game.IsMyTurn)
				return;

			NavigationService.Navigate(new Uri("/BantumiGamePage.xaml?game=" + game.Id, UriKind.Relative));
		}

		public void CreateGame(Object sender, EventArgs e)
		{
			if (Games.Count >= 10)
			{
				MessageBox.Show("You are not allowed to play in more than ten games simultaneously. Please finish some games before starting new ones.");
			}

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
            SystemTray.ProgressIndicator.IsVisible = true;

            Context.PlayerGames(Player.Name, games => 
            {
                Dispatcher.BeginInvoke(delegate 
                {
                    Games.Clear();
                    foreach (var game in games.Select(g => new GameVM(g)))
                        Games.Add(game);

                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    settings["games"] = Games.ToArray();

                    SystemTray.ProgressIndicator.IsVisible = false;
                });
            }, () => 
            {
            });
		}

		public void JoinRandom(Object sender, EventArgs e)
		{
			if (Games.Count >= 10)
			{
				MessageBox.Show("You are not allowed to play in more than ten games simultaneously. Please finish some games before starting new ones.");
			}

			SystemTray.ProgressIndicator.IsVisible = true;
			Context.JoinGame(Player.Name, game =>
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
			}, () =>
			{
				Dispatcher.BeginInvoke(delegate()
				{
					SystemTray.ProgressIndicator.IsVisible = false;
					MessageBox.Show("Failed to join a challenge. Please try again.");
				});
			});
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