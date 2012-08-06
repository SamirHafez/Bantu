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
using Microsoft.Phone.Notification;

namespace Bantu.Notification
{
	public static class Manager
	{
		private const string CHANNEL = "bantuToastNotification";
		private static HttpNotificationChannel _channel;
		private static string _username;

        public static event Action<string> GameEventToast = delegate { };

		public static void EnableNotifications(string username)
		{
			_username = username;
			if (_channel == null)
			{
				_channel = HttpNotificationChannel.Find(CHANNEL);

				if (_channel == null)
				{
					_channel = new HttpNotificationChannel(CHANNEL);
					WireChannel(_channel);
					_channel.Open();
				}
				else
					WireChannel(_channel);

				if (!_channel.IsShellToastBound)
					_channel.BindToShellToast();

				if (_channel.ChannelUri != null)
				{
					var ns = new NotificationServiceClient();
					ns.RegisterEndpointAsync(username, _channel.ChannelUri.ToString());
				}
			}
		}

		public static void DisableNotification() 
		{
			_channel = null;
			var ns = new NotificationServiceClient();
			ns.UnregisterEndpointAsync(_username);
		}

		private static void WireChannel(HttpNotificationChannel channel)
		{
			channel.ChannelUriUpdated -= ChannelUpdated;
			channel.ChannelUriUpdated += ChannelUpdated;

			channel.ShellToastNotificationReceived -= ToastReceived;
			channel.ShellToastNotificationReceived += ToastReceived;

			channel.ErrorOccurred -= ErrorOccurred;
			channel.ErrorOccurred += ErrorOccurred;
		}

		private static void ChannelUpdated(object sender, NotificationChannelUriEventArgs args) 
		{
			var ns = new NotificationServiceClient();
			ns.RegisterEndpointAsync(_username, args.ChannelUri.ToString());
		}

		private static void ToastReceived(object sender, NotificationEventArgs args)
		{
            var param = args.Collection["wp:Param"];
            var gameId = param.Substring(param.IndexOf("=")+1);
            GameEventToast(gameId);
		}

		private static void ErrorOccurred(object sender, NotificationChannelErrorEventArgs args)
		{
		}
	}
}
