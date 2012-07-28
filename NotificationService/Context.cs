using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using NotificationService.Model;

namespace NotificationService
{
	internal class Context : TableServiceContext
	{
		private static CloudStorageAccount account;
		public const string PLAYER = "player";

		static Context()
		{
			CloudStorageAccount.SetConfigurationSettingPublisher((configname, configsettingsPublisher) => 
			{
				var connectionString = RoleEnvironment.GetConfigurationSettingValue(configname);
				configsettingsPublisher(connectionString);
			});
			account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
			CloudTableClient tableClient = new CloudTableClient(account.TableEndpoint.AbsoluteUri, account.Credentials);
		}

		public Context()
			: base(account.TableEndpoint.AbsoluteUri, account.Credentials)
		{
		}

		public IQueryable<Player> Player
		{
			get { return this.CreateQuery<Player>(PLAYER); }
		}
	}
}