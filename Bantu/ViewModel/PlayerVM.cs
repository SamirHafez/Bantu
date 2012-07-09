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

namespace Bantu.ViewModel
{
    public class PlayerVM
    {
        public string Name { get; set; }
        public string Credential { get; set; }
        public long Score { get; set; }

        public PlayerVM() { }

        public PlayerVM(Player player) 
        {
            Name = player.Name;
            Credential = player.Credential;
            Score = player.Score;
        }
    }
}
