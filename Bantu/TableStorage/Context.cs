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
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Bantu.ViewModel;
using Bantu.TableStorage;

namespace Bantu.TableStorage
{
    internal static class Context
    {
        public static void Reset()
        {
			var sc = new StorageServiceClient();
			sc.ResetAsync();
        }

        public static void CreatePlayer(string username, string identifier, Action<Player> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.CreatePlayerCompleted += (o, a) => 
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.CreatePlayerAsync(username, identifier);
        }

        public static void CreateGame(string host, Action<Game> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.CreateGameCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.CreateGameAsync(host);
        }

		public static void GetPlayerByName(string username, Action<Player> success, Action failure)
		{
			var sc = new StorageServiceClient();
			sc.GetPlayerByNameCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.GetPlayerByNameAsync(username);
		}

		public static void GetPlayerByIdentifier(string identifier, Action<Player> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.GetPlayerByIdentifierCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.GetPlayerByIdentifierAsync(identifier);
        }

        public static void PlayerGames(string username, Action<IEnumerable<Game>> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.PlayerGamesCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.PlayerGamesAsync(username);
        }

        public static void GetGame(string gameId, Action<Game> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.GetGameCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.GetGameAsync(gameId);
        }

        public static void UpdateGame(string playerId, Game game, Action<Game> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.UpdateGameCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.UpdateGameAsync(playerId, game);
        }

        public static void ScorePlayer(string playerId, int addedScore, Action<Player> success, Action failure)
        {
			var sc = new StorageServiceClient();
			sc.ScorePlayerCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.ScorePlayerAsync(playerId, addedScore);
        }

        public static void JoinGame(string username, Action<Game> success, Action failure, string gameId = null)
        {
			var sc = new StorageServiceClient();
			sc.JoinGameCompleted += (o, a) =>
			{
				if (a.Error == null)
					success(a.Result);
				else
					failure();
			};
			sc.JoinGameAsync(username, gameId);
        }
    }
}
