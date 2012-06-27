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
using Bantu.Model;
using Bantu.Helpers;
using Microsoft.Phone.Shell;

namespace Bantu
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Player Me { get; set; }
        private Game CurrentGame { get; set; }

        public GamePage()
        {
            InitializeComponent();
        }

        public void Setup(object sender, EventArgs e) 
        {
            var gameIndex = int.Parse(NavigationContext.QueryString["game"]);
            CurrentGame = MainPage.ActiveGames[gameIndex];

            DataContext = CurrentGame;
            Me = MainPage.Player;

            PageTitle.Text = CurrentGame.Turn.Equals(Me.Name) ? "Your turn" : "Waiting on " + (CurrentGame.Host.Equals(Me.Name) ? CurrentGame.Client : CurrentGame.Host);

            SystemTray.ProgressIndicator.IsVisible = true;

            ModelHelpers.NewPlays(CurrentGame.RowKey, plays => 
            {
                foreach (var play in plays)
                    UpdateGame(play);

                SystemTray.ProgressIndicator.IsVisible = false;
            }, () => 
            {
                SystemTray.ProgressIndicator.IsVisible = false;
            });
        }

        public void Play(object sender, GestureEventArgs e) 
        {
            var currentGame = (Game)((FrameworkElement)e.OriginalSource).DataContext;

            ////GET THE CUP WITH PLAYER INDEX-BASED
            //if (currentCount == 0) return GameState;

            ////PLACE EACH STONE ON THE NEXT CUP (SKIP OPONENT SCORE CUP)
            //var stones = cup.Stones;
            //cup.Stones = 0;

            //Cup nextCup = cup;
            //do
            //{
            //    nextCup = Board.Next(nextCup);

            //    if (nextCup.IsScore && nextCup.Owner != current)
            //        continue;

            //    nextCup.Stones++;
            //    stones--;
            //} while (stones != 0);

            ////IF END IN SCORE CUP, PLAY AGAIN (NO CHANGES TO GAMESTATE)
            //if (nextCup.IsScore)
            //    return GameState;

            ////IF END IN EMPTY CUP, SCORE 1 + OPONENT CUP
            //if (nextCup.Stones == 1)
            //{
            //    var opositeCup = Board.Oposite(nextCup);
            //    Board.Score(current).Stones += nextCup.Stones + opositeCup.Stones;
            //    nextCup.Stones = opositeCup.Stones = 0;
            //}

            ////IF GAME ENDED, REPORT SO
            //if (Board.CupCount(current) == 0 || Board.CupCount(other) == 0)
            //{
            //    Board.Collect();
            //    return GameState = GameStateEnum.Finished;
            //}

            ////SWITCH THE PLAY
            //return GameState = GameState == GameStateEnum.Player1 ? GameStateEnum.Player2 : GameStateEnum.Player1;
        }

        //public void Play(int index) 
        //{
        //    SystemTray.ProgressIndicator.IsVisible = true;

        //    ModelHelpers.Play(Me.Name, CurrentGame.RowKey, index, play =>
        //    {
        //        UpdateGame(play);

        //        SystemTray.ProgressIndicator.IsVisible = false;
        //    }, () => 
        //    {
        //        SystemTray.ProgressIndicator.IsVisible = false;
        //    });
        //}

        private void UpdateGame(Play play)
        {
        }
    }
}