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
using System.Runtime.Serialization;

namespace Bantu.Model
{
    [DataContract]
    [DataServiceEntity]
    [EntitySet("Player")]
    public class Player : TableServiceEntity
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Credential { get; set; }

        [DataMember]
        public long Score { get; set; }

        public Player()
        {
        }

        public Player(string username, string credential) : base("player", username)
        {
            Name = username;
            Credential = credential;
        }
    }
}
