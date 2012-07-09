using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Samples.Phone.Storage;
using Microsoft.WindowsAzure.Samples.Data.Services.Client;

namespace Bantu.Azure.Model
{
    [DataServiceEntity]
    [EntitySet("Game")]
    public class Game : TableServiceEntity
    {
        public string Host { get; set; }
        public string Client { get; set; }

        public bool HostTurn { get; set; }

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

        public Game(string hostUsername) : base("game", DateTime.Now.Ticks.ToString())
        {
            Host = hostUsername;
            Client = string.Empty;
            HostTurn = false;

            Host0 = Host1 = Host2 = Host3 = Host4 = Host5 = 4;
            Client0 = Client1 = Client2 = Client3 = Client4 = Client5 = 4;
        }
    }
}
