using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using StorageService.Model;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace StorageService
{
	public class Context : TableServiceContext
	{
		private static CloudStorageAccount account;
		public const string PLAYER = "player";
		public const string GAME = "game";

		static Context()
		{
			CloudStorageAccount.SetConfigurationSettingPublisher((configname, configsettingsPublisher) => 
			{
				var connectionString = RoleEnvironment.GetConfigurationSettingValue(configname);
				configsettingsPublisher(connectionString);
			});
			account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
			CloudTableClient tableClient = new CloudTableClient(account.TableEndpoint.AbsoluteUri, account.Credentials);
            //tableClient.DeleteTableIfExist(PLAYER);
            //tableClient.DeleteTableIfExist(GAME);
            tableClient.CreateTableIfNotExist(PLAYER);
            tableClient.CreateTableIfNotExist(GAME);
		}

		public Context()
			: base(account.TableEndpoint.AbsoluteUri, account.Credentials)
		{
		}

		public IQueryable<Player> Player
		{
			get { return this.CreateQuery<Player>(PLAYER); }
		}

		public IQueryable<Game> Game
		{
			get { return this.CreateQuery<Game>(GAME); }
		}
	}
}