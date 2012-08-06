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
		Player CreatePlayer(string username, string identifier);

		[OperationContract]
		Game CreateGame(string host);

		[OperationContract]
		Player GetPlayerByName(string username);

		[OperationContract]
		Player GetPlayerByIdentifier(string nameIdentifier);

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
