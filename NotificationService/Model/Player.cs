using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace NotificationService.Model
{
	public class Player : TableServiceEntity
	{
		public string Identifier { get; set; }
		public long Score { get; set; }

		public string Endpoint { get; set; }

		public Player()
		{
		}

		public Player(string username, string identifier)
			: base(DateTime.Now.Day.ToString(), username)
		{
			Identifier = identifier;
		}
	}
}
