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
using Bantu.ViewModel;

namespace Bantu.Azure
{
    public static class Context
    {
        private static readonly ICloudTableClient TableClient = CloudStorageContext.Current.Resolver.CreateCloudTableClient();
        private const string PLAYERS = "player";
        private const string GAME = "game";

        public static void Reset()
        {
            TableClient.DeleteTableIfExist(PLAYERS, cor =>
            {
            });
            TableClient.DeleteTableIfExist(GAME, cor =>
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

        public static void ValidatePlayer(string username, string password, Action<Player> success, Action failure)
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

        public static void PlayerGames(string username, Action<IEnumerable<Game>> success, Action failure)
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
                "{0}/{1}?$filter=Host eq '{2}' or Client eq '{2}' and State ne '2'",
                context.BaseUri,
                GAME,
                username),
                UriKind.Absolute);

            openGames.Clear();
            openGames.LoadAsync(uri);
        }

        public static void GetGame(string gameId, Action<Game> success, Action failure, DateTime lastGet)
        {
            var context = TableClient.GetDataServiceContext();

            var games = new DataServiceCollection<Game>(context);
            games.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                {
                    var game = games.FirstOrDefault(g => g.Timestamp > lastGet);
                    if (game != null)
                        success(game);
                    else
                        failure();
                }
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top=1&$filter=RowKey eq '{2}'",
                context.BaseUri,
                GAME,
                gameId),
                UriKind.Absolute);

            games.Clear();
            games.LoadAsync(uri);
        }

        public static void UpdateGame(GameVM game, Action<Game> success, Action failure)
        {
            var context = TableClient.GetDataServiceContext();

            var games = new DataServiceCollection<Game>(context);
            games.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                {
                    var g = games.ToList().FirstOrDefault();

                    g.Client0 = game.Cups[7].Stones;
                    g.Client1 = game.Cups[8].Stones;
                    g.Client2 = game.Cups[9].Stones;
                    g.Client3 = game.Cups[10].Stones;
                    g.Client4 = game.Cups[11].Stones;
                    g.Client5 = game.Cups[12].Stones;

                    g.Host0 = game.Cups[0].Stones;
                    g.Host1 = game.Cups[1].Stones;
                    g.Host2 = game.Cups[2].Stones;
                    g.Host3 = game.Cups[3].Stones;
                    g.Host4 = game.Cups[4].Stones;
                    g.Host5 = game.Cups[5].Stones;

                    g.ScoreClient = game.Cups[6].Stones;
                    g.ScoreHost = game.Cups[13].Stones;

                    g.State = (int)game.State;

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
                                failure();
                            }
                        }
                        , null);
                }
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top=1&$filter=RowKey eq '{2}'",
                context.BaseUri,
                GAME,
                game.Id),
                UriKind.Absolute);

            games.Clear();
            games.LoadAsync(uri);
        }

        public static void ScorePlayer(string playerId, int addedScore, Action<Player> success, Action failure)
        {
            var context = TableClient.GetDataServiceContext();

            var players = new DataServiceCollection<Player>(context);
            players.LoadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    failure();
                else
                {
                    var p = players.ToList().FirstOrDefault();

                    p.Score += addedScore;

                    context.UpdateObject(p);

                    context.BeginSaveChanges(
                        asyncResult =>
                        {
                            DataServiceResponse response;
                            try
                            {
                                response = context.EndSaveChanges(asyncResult);

                                //TODO CALL SUCCESS OR FAILURE BASED ON RESPONSE
                                success(p);
                            }
                            catch (Exception)
                            {
                                failure();
                            }
                        }
                        , null);
                }
            };

            var uri = new Uri(
                string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}?$top=1&$filter=RowKey eq '{2}'",
                context.BaseUri,
                PLAYERS,
                playerId),
                UriKind.Absolute);

            players.Clear();
            players.LoadAsync(uri);
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
                "{0}/{1}?$top=1&$filter=RowKey eq '{2}'",
                context.BaseUri,
                GAME,
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
