using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Bantu.Azure.Model;
using System.Linq;
using System.ComponentModel;

namespace Bantu.ViewModel
{
    public enum GameState
    {
        Host,
        Client,
        Finished
    }

    public class GameVM : INotifyPropertyChanged
    {
        public string Id { get; set; }

        private PlayerVM _host;
        public PlayerVM Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                foreach (var cup in Cups.Take(Cups.Length / 2))
                    cup.Owner = _host;
            }
        }

        private PlayerVM _client;
        public PlayerVM Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                foreach (var cup in Cups.Skip(Cups.Length / 2))
                    cup.Owner = _client;
            }
        }

        public string Turn { get { return State == GameState.Host ? Host.Name : Client.Name; } }

        public DateTime LastUpdate { get; set; }

        private GameState _gameState;
        public GameState State 
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Turn"));
            }
        }

        public CupVM[] Cups { get; set; }

        public GameVM()
        {
            Cups = new CupVM[14];
            for (int i = 0; i < Cups.Length; ++i)
                Cups[i] = new CupVM { Stones = 4, Index = i };

            Cups.Last().IsScore = Cups.Take(Cups.Length / 2).Last().IsScore = true;
            Cups.Last().Stones = Cups.Take(Cups.Length / 2).Last().Stones = 0;

            State = GameState.Client;
        }

        public GameVM(Game game)
            : this()
        {
            Id = game.RowKey;

            Host = new PlayerVM { Name = game.Host };
            Client = new PlayerVM { Name = game.Client };

            Cups[0].Stones = game.Host0;
            Cups[1].Stones = game.Host1;
            Cups[2].Stones = game.Host2;
            Cups[3].Stones = game.Host3;
            Cups[4].Stones = game.Host4;
            Cups[5].Stones = game.Host5;

            Cups[7].Stones = game.Client0;
            Cups[8].Stones = game.Client1;
            Cups[9].Stones = game.Client2;
            Cups[10].Stones = game.Client3;
            Cups[11].Stones = game.Client4;
            Cups[12].Stones = game.Client5;

            State = (GameState)game.State;

            Cups.Last().Stones = game.ScoreHost;
            Cups.Take(Cups.Length / 2).Last().Stones = game.ScoreClient;

            LastUpdate = game.Timestamp;
        }

        public void Update(Game game) 
        {
            Cups[0].Stones = game.Host0;
            Cups[1].Stones = game.Host1;
            Cups[2].Stones = game.Host2;
            Cups[3].Stones = game.Host3;
            Cups[4].Stones = game.Host4;
            Cups[5].Stones = game.Host5;

            Cups[7].Stones = game.Client0;
            Cups[8].Stones = game.Client1;
            Cups[9].Stones = game.Client2;
            Cups[10].Stones = game.Client3;
            Cups[11].Stones = game.Client4;
            Cups[12].Stones = game.Client5;

            State = (GameState)game.State;

            Cups.Last().Stones = game.ScoreHost;
            Cups.Take(Cups.Length / 2).Last().Stones = game.ScoreClient;

            LastUpdate = game.Timestamp;
        }

        public CupVM Get(PlayerVM current, int cupIndex)
        {
            return Cups.Where(c => c.Owner.Name == current.Name).ElementAt(cupIndex);
        }

        public CupVM Score(PlayerVM current)
        {
            return Cups.Single(c => c.IsScore && c.Owner.Name == current.Name);
        }

        public bool Play(int idx) 
        {
            return Play(Cups[idx]);
        }

        public bool Play(CupVM cup)
        {
            //GET THE CURRENT AND OPONENT PLAYERS
            PlayerVM current = State == GameState.Host ? Host : Client;
            PlayerVM other = State == GameState.Client ? Client : Host;

            if (cup.Owner.Name != current.Name) return false;

            //GET THE CUP WITH PLAYER INDEX-BASED
            if (cup.Stones == 0) return false;

            //PLACE EACH STONE ON THE NEXT CUP (SKIP OPONENT SCORE CUP)
            var stones = cup.Stones;
            cup.Stones = 0;

            CupVM nextCup = cup;
            do
            {
                nextCup = Next(nextCup);

                if (nextCup.IsScore && nextCup.Owner != current)
                    continue;

                nextCup.Stones++;
                stones--;
            } while (stones != 0);

            //IF END IN SCORE CUP, PLAY AGAIN (NO CHANGES TO GAMESTATE)
            if (nextCup.IsScore)
                return true;

            //IF END IN EMPTY CUP, SCORE 1 + OPONENT CUP
            if (nextCup.Stones == 1)
            {
                var opositeCup = Oposite(nextCup);
                Score(current).Stones += nextCup.Stones + opositeCup.Stones;
                nextCup.Stones = opositeCup.Stones = 0;
            }

            //IF GAME ENDED, REPORT SO
            if (CupCount(current) == 0 || CupCount(other) == 0)
            {
                Collect();
                State = GameState.Finished;
                return true;
            }

            //SWITCH THE PLAY
            State = State == GameState.Host ? GameState.Client : GameState.Host;
            return true;
        }

        private void Collect()
        {
            var groups = Cups.Where(c => !c.IsScore).GroupBy(c => c.Owner);

            foreach (var group in groups)
            {
                Score(group.Key).Stones += CupCount(group.Key);
                foreach (var cup in group)
                    cup.Stones = 0;
            }
        }

        private CupVM Next(CupVM cup)
        {
            return Cups.SingleOrDefault(c => c.Index == cup.Index + 1) ?? Cups.First();
        }

        private CupVM Oposite(CupVM cup)
        {
            var count = Cups.Count();
            return Cups.Single(c => c.Index == (count - 2) - cup.Index);
        }

        private int CupCount(PlayerVM p)
        {
            return Cups.Where(c => c.Owner.Name == p.Name && !c.IsScore).Sum(c => c.Stones);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
