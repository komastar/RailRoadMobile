using System;
using System.Threading;
using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Manager;
using NUnit.Framework;

namespace Tests.RESTAPI.Game
{
    public class GameTest
    {
        [Test]
        public void T_001_FullSuc()
        {
            UrlTable.IsRemote = false;
            GameRoomModel game = CreateGameTest(4);
            Assert.AreNotEqual(null, game);
            Assert.AreEqual(false, string.IsNullOrEmpty(game.GameCode));
            Assert.AreEqual(4, game.GameCode.Length);
            Assert.AreEqual(null, ExitGameTest(game));
        }

        [Test]
        public void T_002_JoinFail()
        {
            UrlTable.IsRemote = false;
            Assert.AreEqual(null, JoinGameTest("****"));
        }

        [Test]
        public void T_003_ExitFail()
        {
            UrlTable.IsRemote = false;
            Assert.AreEqual(null, ExitGameTest(new GameRoomModel("****", Guid.NewGuid().ToString())));
        }

        [Test]
        public void T_004_JoinOverCap()
        {
            UrlTable.IsRemote = false;
            var game = CreateGameTest(4);
            Assert.AreNotEqual(null, game);
            Assert.AreEqual(4, game.GameCode.Length);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreNotEqual(null, JoinGameTest(game.GameCode));
            }
            Assert.AreEqual(null, JoinGameTest(game.GameCode));
            Assert.AreEqual(true, DeleteGameTest(game));
        }

        private GameRoomModel CreateGameTest(int userCount)
        {
            Log.Info($"CREATE {userCount}");
            var newGame = NetworkManager.CreateGame(userCount);
            Log.Info($"CREATED {newGame.GameCode}");

            return newGame;
        }

        private GameRoomModel JoinGameTest(string gameCode)
        {
            Log.Info($"JOIN {gameCode}");
            return NetworkManager.JoinGame(gameCode);
        }

        private GameRoomModel ExitGameTest(GameRoomModel gameRoom)
        {
            Log.Info($"EXIT {gameRoom.GameCode}");
            return NetworkManager.ExitGame(gameRoom.GameCode, gameRoom.UserId);
        }

        private bool DeleteGameTest(GameRoomModel gameRoom)
        {
            Log.Info($"DELETE {gameRoom.GameCode}");
            return NetworkManager.DeleteGame(gameRoom.GameCode);
        }
    }
}
