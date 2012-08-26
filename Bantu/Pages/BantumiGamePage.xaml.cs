using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Bantu.ViewModel;
using Bantu.Controls;
using Microsoft.Phone.Shell;
using Bantu.TableStorage;
using System.IO.IsolatedStorage;
using Bantu.Notification;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Bantu.Pages
{
    public partial class BantumiGamePage : PhoneApplicationPage
    {
        public GameVM Game { get; private set; }
        public PlayerVM Player { get; set; }

        private NotificationServiceClient _nsc;

        public BantumiGamePage()
        {
            InitializeComponent();
            _nsc = new NotificationServiceClient();
        }

        public void Setup(object sender, EventArgs args)
        {
            var gameId = NavigationContext.QueryString["game"];
            var settings = IsolatedStorageSettings.ApplicationSettings;
            Player = settings["player"] as PlayerVM;

            Context.GetGame(gameId, game =>
                Dispatcher.BeginInvoke(() =>
                {
                    Game = new GameVM(game);

                    if (Game.Host == Player)
                        BindHost();
                    else
                        BindClient();

                    BindChangingProperties();
                    BindChangedProperties();
                })
            , () => { });
        }

        public void Play(object sender, GestureEventArgs gestureEventArgs)
        {
            var cup = ((CupControl)sender).DataContext as CupVM;

            var currentGameState = Game.State;
            if (!Game.Play(cup))
                return;
            SystemTray.ProgressIndicator.IsVisible = true;
            Context.UpdateGame(Player.Name, Game.ToGame(), game =>
                Dispatcher.BeginInvoke(() =>
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    switch (Game.State)
                    {
                        case GameState.Finished:
                            var winner = Game.Winner;
                            var score = Game.Score(winner).Stones;
                            if (winner == Player)
                            {
                                MessageBox.Show("Congratulations, you won! You have earned " + score + " points.");
                                Context.ScorePlayer(winner.Name, score, player =>
                                    Dispatcher.BeginInvoke(NavigationService.GoBack)
                                , () => { });
                            }
                            else
                            {
                                MessageBox.Show("Sorry, you lost.");
                                Context.ScorePlayer(winner.Name, score, player =>
                                    Dispatcher.BeginInvoke(NavigationService.GoBack)
                                , () => { });
                            }
                            break;
                        case GameState.Client:
                            if (currentGameState != GameState.Client)
                                _nsc.NotifyAsync(Player.Name, Game.Client.Name, Game.Id);
                            else
                                CupControl.AnimationDelayIndex = 0;
                            break;
                        case GameState.Host:
                            if (currentGameState != GameState.Host)
                                _nsc.NotifyAsync(Player.Name, Game.Host.Name, Game.Id);
                            else
                                CupControl.AnimationDelayIndex = 0;
                            break;
                    }
                }), () =>
                Dispatcher.BeginInvoke(() =>
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("Failed to register play. Try again.");
                })
            );
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

        private void BindChangedProperties()
        {
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

        private void BindChangingProperties()
        {
            ((CupVM)ccP10.DataContext).PropertyChanging += ccP10.Preset;
            ((CupVM)ccP11.DataContext).PropertyChanging += ccP11.Preset;
            ((CupVM)ccP12.DataContext).PropertyChanging += ccP12.Preset;
            ((CupVM)ccP13.DataContext).PropertyChanging += ccP13.Preset;
            ((CupVM)ccP14.DataContext).PropertyChanging += ccP14.Preset;
            ((CupVM)ccP15.DataContext).PropertyChanging += ccP15.Preset;
            ((CupVM)ccP1Score.DataContext).PropertyChanging += ccP1Score.Preset;

            ((CupVM)ccP20.DataContext).PropertyChanging += ccP20.Preset;
            ((CupVM)ccP21.DataContext).PropertyChanging += ccP21.Preset;
            ((CupVM)ccP22.DataContext).PropertyChanging += ccP22.Preset;
            ((CupVM)ccP23.DataContext).PropertyChanging += ccP23.Preset;
            ((CupVM)ccP24.DataContext).PropertyChanging += ccP24.Preset;
            ((CupVM)ccP25.DataContext).PropertyChanging += ccP25.Preset;
            ((CupVM)ccP2Score.DataContext).PropertyChanging += ccP2Score.Preset;
        }
    }
}