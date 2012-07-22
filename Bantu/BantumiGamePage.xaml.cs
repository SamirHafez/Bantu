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

            ((CupVM)ccP10.DataContext).PropertyChanged += ccP10.Animate;
            ((CupVM)ccP11.DataContext).PropertyChanged += ccP11.Animate;
            ((CupVM)ccP12.DataContext).PropertyChanged += ccP12.Animate;
            ((CupVM)ccP13.DataContext).PropertyChanged += ccP13.Animate;
            ((CupVM)ccP14.DataContext).PropertyChanged += ccP14.Animate;
            ((CupVM)ccP15.DataContext).PropertyChanged += ccP15.Animate;
            ((CupVM)ccP1Score.DataContext).PropertyChanged += ccP1Score.Animate;

            ((CupVM)ccP20.DataContext).PropertyChanged += ccP20.Animate;
            ((CupVM)ccP21.DataContext).PropertyChanged += ccP21.Animate;
            ((CupVM)ccP22.DataContext).PropertyChanged += ccP22.Animate;
            ((CupVM)ccP23.DataContext).PropertyChanged += ccP23.Animate;
            ((CupVM)ccP24.DataContext).PropertyChanged += ccP24.Animate;
            ((CupVM)ccP25.DataContext).PropertyChanged += ccP25.Animate;
            ((CupVM)ccP2Score.DataContext).PropertyChanged += ccP2Score.Animate;
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
                                var score = Game.Score(winner).Stones;
                                if (winner.Name == Player.Name)
                                {
                                    MessageBox.Show("Congratulations, you won! You have earned " + score + " points.");
                                    Context.ScorePlayer(Player.Name, score, player =>
                                    {
                                        Dispatcher.BeginInvoke(delegate()
                                        {
                                            MainPage.Player.Score = player.Score;
                                            NavigationService.GoBack();
                                        });
                                    }, () => { });
                                }
                                else
                                {
                                    MessageBox.Show("Sorry, you lost.");
                                    Context.ScorePlayer(winner.Name, score, player =>
                                    {
                                        Dispatcher.BeginInvoke(delegate()
                                        {
                                            NavigationService.GoBack();
                                        });
                                    }, () => { });
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
    }
}