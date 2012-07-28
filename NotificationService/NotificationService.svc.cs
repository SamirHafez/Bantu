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
            var player = (from n in _context.Player
                          where n.RowKey == username
                          select n).FirstOrDefault();

            if (player == null)
                return;

            player.Endpoint = endpoint;

            _context.UpdateObject(player);

            _context.SaveChanges();
        }

        public void UnregisterEndpoint(string username)
        {
            var player = (from n in _context.Player
                          where n.RowKey == username
                          select n).FirstOrDefault();

            if (player == null)
                return;

            player.Endpoint = string.Empty;

            _context.UpdateObject(player);
            _context.SaveChanges();
        }

        public void Notify(string from, string to, string gameId)
        {
            var player = (from n in _context.Player
                          where n.RowKey == to
                          select n).FirstOrDefault();

            if (player == null)
                return;

            if (player.Endpoint != string.Empty)
                SendNotification(from, "Your move!", gameId, player.Endpoint);
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
