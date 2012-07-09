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
