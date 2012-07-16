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
using Microsoft.WindowsAzure.Samples.Phone.Storage;
using Bantu.Azure.Model;
using Microsoft.WindowsAzure.Samples.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace Bantu.Azure
{
    public static class Context
    {
        private static readonly ICloudTableClient TableClient = CloudStorageContext.Current.Resolver.CreateCloudTableClient();
        private const string PLAYERS = "player";
        private const string GAME = "game";
        private const string PLAY = "play";

        public static void Reset()
        {
            TableClient.DeleteTableIfExist(PLAYERS, cor =>
            {
            });
            TableClient.DeleteTableIfExist(GAME, cor =>
            {
            });
            TableClient.DeleteTableIfExist(PLAY, cor =>
            {
            });
        }

        public static void CreatePlayer(string username, string credential, Action<Player> success, Action failure)
        {
            var player = new Player(username, credential);

            CreateEntity(player, PLAYERS, success, failure);
        }

        public static void CreateGame(string host, Action<Game> success, Action failure)
        {
            var game = new Game(host);

            CreateEntity(game, GAME, success, failure);
        }

        public static void Play(string gameId, string username, int idx, Action<Play> success, Action failure)
        {
            var play = new Play(username, gameId, idx);

            CreateEntity(play, PLAY, success, failure);
        }

        public static void Login(string username, string password, Action<Player> success, Action failure)
        {
            var context = TableClient.GetDataServiceContext();

            var foundPlayers = new DataServiceCollection<Player>(context);
            foundPlayers.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                {
                    var player = foundPlayers.ToList().FirstOrDefault();
                    if (player == null)
                        failure();
                    else
                        success(player);
                }
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top=1&$filter=(Name eq '{2}') and (Credential eq '{3}')",
                context.BaseUri,
                PLAYERS,
                username,
                password),
                UriKind.Absolute);

            foundPlayers.Clear();
            foundPlayers.LoadAsync(uri);
        }

        public static void OpenGames(string username, Action<IEnumerable<Game>> success, Action failure, int amount = 10)
        {
            var context = TableClient.GetDataServiceContext();

            var openGames = new DataServiceCollection<Game>(context);
            openGames.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                    success(openGames.ToList());
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top={2}&$filter=Host ne '{3}' and Client eq ''",
                context.BaseUri,
                GAME,
                amount,
                username),
                UriKind.Absolute);

            openGames.Clear();
            openGames.LoadAsync(uri);
        }

        public static void GetGame(string gameId, Action<Game> success, Action failure)
        {
            var context = TableClient.GetDataServiceContext();

            var games = new DataServiceCollection<Game>(context);
            games.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                    success(games.FirstOrDefault());
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top=1&$filter=PartitionKey eq '{2}' and RowKey eq '{3}'",
                context.BaseUri,
                GAME,
                GAME,
                gameId),
                UriKind.Absolute);

            games.Clear();
            games.LoadAsync(uri);
        }

        public static void JoinGame(string username, Game game, Action<Game> success, Action<Game> failure)
        {
            var context = TableClient.GetDataServiceContext();

            var games = new DataServiceCollection<Game>(context);
            games.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure(game);
                else
                {
                    var g = games.ToList().FirstOrDefault();
                    g.Client = username;
                    context.UpdateObject(g);

                    context.BeginSaveChanges(
                        asyncResult =>
                        {
                            DataServiceResponse response;
                            try
                            {
                                response = context.EndSaveChanges(asyncResult);

                                //TODO CALL SUCCESS OR FAILURE BASED ON RESPONSE
                                success(g);
                            }
                            catch (Exception)
                            {
                                failure(game);
                            }
                        }
                        , null);
                }
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top=1&$filter=PartitionKey eq '{2}' and RowKey eq '{3}'",
                context.BaseUri,
                GAME,
                game.PartitionKey,
                game.RowKey),
                UriKind.Absolute);

            games.Clear();
            games.LoadAsync(uri);
        }

        private static void CreateEntity<T>(T entity, string table, Action<T> success, Action failure)
        {
            TableClient.CreateTableIfNotExist(
                table,
                cor =>
                {
                    var context = TableClient.GetDataServiceContext();

                    context.AddObject(table, entity);
                    context.BeginSaveChanges(
                        asyncResult =>
                        {
                            DataServiceResponse response;
                            try
                            {
                                response = context.EndSaveChanges(asyncResult);

                                success(entity);
                            }
                            catch
                            {
                                failure();
                            }
                        }
                        , null);
                }
                );
        }
    }
}
