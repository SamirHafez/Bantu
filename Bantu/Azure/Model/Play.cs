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
using Microsoft.WindowsAzure.Samples.Data.Services.Client;
using Microsoft.WindowsAzure.Samples.Phone.Storage;

namespace Bantu.Azure.Model
{
    [DataServiceEntity]
    [EntitySet("Play")]
    public class Play : TableServiceEntity
    {
        public string Username { get; set; }
        public string GameId { get; set; }

        public int Index { get; set; }

        public Play()
        {
        }

        public Play(string username, string gameRow, int index)
            : base("play", DateTime.Now.Ticks.ToString())
        {
            Username = username;
            GameId = gameRow;
            Index = index;
        }
    }
}
