using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using StorageService.Model;

namespace StorageService
{
	[ServiceContract]
	public interface IStorageService
	{
		[OperationContract]
		void Reset();

		[OperationContract]
		Player CreatePlayer(string username, string credential);

		[OperationContract]
		Game CreateGame(string host);

		[OperationContract]
		Player ValidatePlayer(string username, string password);

		[OperationContract]
		IEnumerable<Game> PlayerGames(string username);

		[OperationContract]
		Game GetGame(string gameId);

		[OperationContract]
		Game UpdateGame(string playerId, Game game);

		[OperationContract]
		Player ScorePlayer(string playerId, int addedScore);

		[OperationContract]
		Game JoinGame(string username, string gameId = null);
	}
}
