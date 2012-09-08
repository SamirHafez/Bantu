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
	public class TableStorageService : IStorageService
	{
		private Context _context;

		public TableStorageService()
		{
			_context = new Context();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public Player CreatePlayer(string username, string identifier)
		{
			if ((from p in _context.Player
				 where p.RowKey == username
				 select p).FirstOrDefault() != null)
				throw new Exception("Duplicate username");

			var player = new Player(username, identifier);
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

		public Player GetPlayerByName(string username)
		{
			return (from p in _context.Player
					where p.RowKey == username
					select p).First();
		}

		public Player GetPlayerByIdentifier(string nameIdentifier)
		{
			return (from p in _context.Player
					where p.Identifier == nameIdentifier
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

			return g;
		}

		public Player ScorePlayer(string playerId, int addedScore)
		{
			var player = (from p in _context.Player
						  where p.RowKey == playerId
						  select p).First();

			player.Score += addedScore;

			_context.UpdateObject(player);
			_context.SaveChanges();

			return player;
		}

		public Game JoinGame(string username, string gameId = null)
		{
			var games = (from g in _context.Game
						 where g.Client == string.Empty && g.Host != username
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
