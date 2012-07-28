using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace StorageService.Model
{
	public enum GameState
	{
		Host,
		Client,
		Finished
	}

    public class Game : TableServiceEntity
    {
        public string Host { get; set; }
        public string Client { get; set; }

        public int State { get; set; }

        public int ScoreHost { get; set; }
        public int ScoreClient { get; set; }

        public int Host0 { get; set; }
        public int Host1 { get; set; }
        public int Host2 { get; set; }
        public int Host3 { get; set; }
        public int Host4 { get; set; }
        public int Host5 { get; set; }

        public int Client0 { get; set; }
        public int Client1 { get; set; }
        public int Client2 { get; set; }
        public int Client3 { get; set; }
        public int Client4 { get; set; }
        public int Client5 { get; set; }

        public Game()
        {
        }

        public Game(string hostUsername)
            : base(DateTime.Now.Day.ToString(), Guid.NewGuid().ToString())
        {
            Host = hostUsername;
            Client = string.Empty;
            State = (int)GameState.Client;

            Host0 = Host1 = Host2 = Host3 = Host4 = Host5 = 4;
            Client0 = Client1 = Client2 = Client3 = Client4 = Client5 = 4;
        }

		public void Update(Game game) 
		{
			Host0 = game.Host0;
			Host1 = game.Host1;
			Host2 = game.Host2;
			Host3 = game.Host3;
			Host4 = game.Host4;
			Host5 = game.Host5;
			Client0 = game.Client0;
			Client1 = game.Client1;
			Client2 = game.Client2;
			Client3 = game.Client3;
			Client4 = game.Client4;
			Client5 = game.Client5;

			ScoreHost = game.ScoreHost;
			ScoreClient = game.ScoreClient;

			State = game.State;

			Host = game.Host;
			Client = game.Client;
		}
    }
}
