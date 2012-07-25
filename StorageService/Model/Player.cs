using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace StorageService.Model
{
    public class Player : TableServiceEntity
    {
        public string Name { get; set; }
        public string Credential { get; set; }
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
