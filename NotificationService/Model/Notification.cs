using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace NotificationService.Model
{
    public class Notification : TableServiceEntity
    {
        public string Player { get; set; }
        public string Endpoint { get; set; }

        public Notification()
        {
        }

        public Notification(string player, string endpoint) : base("notification", player)
        {
            Player = player;
            Endpoint = endpoint;
        }
    }
}
