using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Bantu.ViewModel;
using Bantu.Controls;
using Microsoft.Phone.Shell;
using Bantu.Azure;
using Bantu.Azure.Model;

namespace Bantu
{
    public partial class BantumiGamePage : PhoneApplicationPage
    {
        public GameVM Game { get; private set; }
        public PlayerVM Player { get; set; }

        public BantumiGamePage()
        {
            InitializeComponent();
        }

        public void Setup(object sender, EventArgs args)
        {
            var gameIndex = int.Parse(NavigationContext.QueryString["game"]);
            Game = MainPage.Games[gameIndex];
            Player = MainPage.Player;

            if (Game.Host.Name == Player.Name)
                BindHost();
            else
                BindClient();
        }

        private void BindClient()
        {
            ccP1Score.DataContext = Game.Score(Game.Client);
            ccP10.DataContext = Game.Get(Game.Client, 0);
            ccP11.DataContext = Game.Get(Game.Client, 1);
            ccP12.DataContext = Game.Get(Game.Client, 2);
            ccP13.DataContext = Game.Get(Game.Client, 3);
            ccP14.DataContext = Game.Get(Game.Client, 4);
            ccP15.DataContext = Game.Get(Game.Client, 5);

            ccP2Score.DataContext = Game.Score(Game.Host);
            ccP20.DataContext = Game.Get(Game.Host, 0);
            ccP21.DataContext = Game.Get(Game.Host, 1);
            ccP22.DataContext = Game.Get(Game.Host, 2);
            ccP23.DataContext = Game.Get(Game.Host, 3);
            ccP24.DataContext = Game.Get(Game.Host, 4);
            ccP25.DataContext = Game.Get(Game.Host, 5);
        }

        private void BindHost()
        {
            ccP1Score.DataContext = Game.Score(Game.Host);
            ccP10.DataContext = Game.Get(Game.Host, 0);
            ccP11.DataContext = Game.Get(Game.Host, 1);
            ccP12.DataContext = Game.Get(Game.Host, 2);
            ccP13.DataContext = Game.Get(Game.Host, 3);
            ccP14.DataContext = Game.Get(Game.Host, 4);
            ccP15.DataContext = Game.Get(Game.Host, 5);

            ccP2Score.DataContext = Game.Score(Game.Client);
            ccP20.DataContext = Game.Get(Game.Client, 0);
            ccP21.DataContext = Game.Get(Game.Client, 1);
            ccP22.DataContext = Game.Get(Game.Client, 2);
            ccP23.DataContext = Game.Get(Game.Client, 3);
            ccP24.DataContext = Game.Get(Game.Client, 4);
            ccP25.DataContext = Game.Get(Game.Client, 5);
        }

        public void Play(object sender, EventArgs args)
        {
            var cup = ((CupControl)sender).DataContext as CupVM;

            var currentGameState = Game.State;
            if (Game.Play(cup))
            {
                SystemTray.ProgressIndicator.IsVisible = true;
                Context.UpdateGame(Game, game =>
                {
                    Game.LastUpdate = game.Timestamp;
                    Dispatcher.BeginInvoke(delegate()
                    {
                        SystemTray.ProgressIndicator.IsVisible = false;
                        switch (Game.State)
                        {
                            case GameState.Finished:
                                var winner = Game.Winner;
                                if (winner.Name == Player.Name)
                                {
                                    var score = Game.Score(Player).Stones;
                                    MessageBox.Show("Congratulations, you won! Your account will be credited with " + score);
                                    Context.ScorePlayer(Player.Name, score, player =>
                                    {
                                        Dispatcher.BeginInvoke(delegate()
                                        {
                                            NavigationService.GoBack();
                                        });
                                    }, () => { });
                                }
                                else
                                {
                                    MessageBox.Show("Sorry, you lost.");
                                    NavigationService.GoBack();
                                }
                                break;
                            case GameState.Client:
                                if (currentGameState != GameState.Client)
                                    NavigationService.GoBack();
                                break;
                            case GameState.Host:
                                if (currentGameState != GameState.Host)
                                    NavigationService.GoBack();
                                break;
                        }
                    });
                }, () =>
                {
                    Dispatcher.BeginInvoke(delegate()
                    {
                        SystemTray.ProgressIndicator.IsVisible = false;
                        MessageBox.Show("Failed to register play. Try again.");
                    });
                });
            }
        }
    }
}