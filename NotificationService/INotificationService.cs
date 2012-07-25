using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NotificationService
{
	[ServiceContract]
	public interface INotificationService
	{
		[OperationContract]
		void RegisterEndpoint(string username, string endpoint);

		[OperationContract]
		void UnregisterEndpoint(string username);

		[OperationContract]
		void Notify(string from, string to, string gameId);
	}
}
