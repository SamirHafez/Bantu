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
using Bantu.Model;
using Microsoft.WindowsAzure.Samples.Data.Services.Client;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bantu.Helpers
{
    public static class ModelHelpers
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

        public static void CreateGame(string hostUsername, Action<Game> success, Action failure)
        {
            var game = new Game(hostUsername);

            CreateEntity(game, GAME, success, failure);
        }

        public static void OpenGames(Action<IEnumerable<Game>> success, Action failure, int amount = 10)
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
                "{0}/{1}?$top={2}&$filter=Client eq ''",
                context.BaseUri,
                GAME,
                amount),
                UriKind.Absolute);

            openGames.Clear();
            openGames.LoadAsync(uri);
        }

        //TODO
        public static void NewPlays(string gameRow, Action<IEnumerable<Play>> success, Action failure)
        {
            var context = TableClient.GetDataServiceContext();

            var newPlays = new DataServiceCollection<Play>(context);
            newPlays.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                    success(newPlays.ToList());
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$filter=GameRow eq '{2}'",
                context.BaseUri,
                PLAY,
                gameRow),
                UriKind.Absolute);

            newPlays.Clear();
            newPlays.LoadAsync(uri);
        }

        public static void UpdateGame(Game game, Action<Game> success, Action<Game> failure)
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

                    g.Client0 = game.Client0;
                    g.Client1 = game.Client1;
                    g.Client2 = game.Client2;
                    g.Client3 = game.Client3;
                    g.Client4 = game.Client4;
                    g.Client5 = game.Client5;

                    g.Host0 = game.Host0;
                    g.Host1 = game.Host1;
                    g.Host2 = game.Host2;
                    g.Host3 = game.Host3;
                    g.Host4 = game.Host4;
                    g.Host5 = game.Host5;

                    g.ScoreClient = game.ScoreClient;
                    g.ScoreHost = game.ScoreHost;

                    g.Turn = game.Turn;

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
                    g.Client = g.Turn = username;
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

        public static void Play(string username, string gameRow, int index, Action<Play> success, Action failure) 
        {
            var play = new Play(username, gameRow, index);

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

                                //TODO CALL SUCCESS OR FAILURE BASED ON RESPONSE
                                success(entity);
                            }
                            catch (Exception)
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
