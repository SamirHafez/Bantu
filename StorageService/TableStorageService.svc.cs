﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using StorageService.Model;
using StorageService.Notification;

namespace StorageService
{
	public class TableStorageService : IStorageService
	{
		private Context _context;
		private NotificationServiceClient _notificationService;

		public TableStorageService()
		{
			_context = new Context();
			_notificationService = new NotificationServiceClient();
		}

        public void Reset()
        {
            throw new NotImplementedException();
        }

		public Player CreatePlayer(string username, string credential)
		{
			var player = new Player(username, credential);
			_context.AddObject(Context.PLAYER, player);
			_context.SaveChanges();

			return player;
		}

		public Game CreateGame(string host)
		{
			var game = new Game(host);
			_context.AddObject(Context.GAME, game);
			_context.SaveChanges();

			return game;
		}

		public Player ValidatePlayer(string username, string password)
		{
			return (from p in _context.Player
					where p.Name == username && p.Credential == password
					select p).First();
		}

		public IEnumerable<Game> PlayerGames(string username)
		{
			return (from g in _context.Game
					where (g.Host == username || g.Client == username) && g.State != (int)GameState.Finished
					select g).ToList();
		}

		public Game GetGame(string gameId)
		{
			return (from g in _context.Game
					where g.RowKey == gameId
					select g).First();
		}

		public Game UpdateGame(string player, Game game)
		{
			var g = GetGame(game.RowKey);
			g.Update(game);

			var to = g.Host == player ? g.Client : g.Host;

			_context.UpdateObject(g);
			_context.SaveChanges();

			_notificationService.Notify(player, to, game.RowKey);
			return g;
		}

		public Player ScorePlayer(string playerId, int addedScore)
		{
			var player = (from p in _context.Player
						  where p.Name == playerId
						  select p).First();

			player.Score += addedScore;
			_context.SaveChanges();

			return player;
		}

		public Game JoinGame(string username, string gameId = null)
		{
			var games = (from g in _context.Game
						 where g.Client == string.Empty
						 select g);

			if (gameId != null)
				games = games.Where(g => g.RowKey == gameId);

			var game = games.First();

			game.Client = username;
			_context.UpdateObject(game);
			_context.SaveChanges();

			return game;
		}
    }
}