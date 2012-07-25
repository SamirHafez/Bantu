using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.IO;
using NotificationService.Model;

namespace NotificationService
{
	public class NotificationService : INotificationService
	{
		private Context _context;

		public NotificationService() 
		{
			_context = new Context();
		}

		public void RegisterEndpoint(string username, string endpoint)
		{
			var notification = (from n in _context.Notification
								where n.Player == username
								select n).FirstOrDefault();

			if (notification == null)
				_context.AddObject(Context.NOTIFICATION, new Notification(username, endpoint));
			else 
			{
				notification.Endpoint = endpoint;
				_context.UpdateObject(notification);
			}

			_context.SaveChanges();
		}

		public void UnregisterEndpoint(string username)
		{
			var notification = (from n in _context.Notification
								where n.Player == username
								select n).FirstOrDefault();

			if (notification == null)
				return;

			_context.DeleteObject(notification);
			_context.SaveChanges();
		}

		public void Notify(string from, string to, string gameId)
		{
			var player = (from n in _context.Notification
							where n.Player == to
							select n).FirstOrDefault();

			if (player == null)
				return;

			SendNotification(from, "Made a move", gameId, player.Endpoint);
		}

		private void SendNotification(string title, string message, string gameId, string endpoint)
		{
			string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
									"<wp:Notification xmlns:wp=\"WPNotification\">" +
									"<wp:Toast>" +
									"<wp:Text1>{0}</wp:Text1>" +
									"<wp:Text2>{1}</wp:Text2>" +
									"<wp:Param>/BantumiGamePage.xaml?game={2}</wp:Param>" +
									"</wp:Toast>" +
									"</wp:Notification>";

			toastMessage = string.Format(toastMessage, title, message, gameId);

			byte[] data = Encoding.UTF8.GetBytes(toastMessage);

			var request = (HttpWebRequest)WebRequest.Create(endpoint);
			request.Method = WebRequestMethods.Http.Post;
			request.ContentType = "text/xml";
			request.ContentLength = data.Length;
			request.Headers.Add("X-MessageID", Guid.NewGuid().ToString());

			request.Headers["X-WindowsPhone-Target"] = "toast";
			request.Headers.Add("X-NotificationClass", "2");

			using (Stream requestStream = request.GetRequestStream())
				requestStream.Write(data, 0, data.Length);

			request.GetResponse();
		}
	}
}
