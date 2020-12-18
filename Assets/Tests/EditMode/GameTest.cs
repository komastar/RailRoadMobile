using System;
using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Manager;
using NUnit.Framework;

namespace Tests.RESTAPI.Game
{
    public class GameTest
    {
        [SetUp]
        public void Setup()
        {
            UrlTable.IsRemote = false;
        }

        [Test]
        public void T_001_FullSuc()
        {
            GameRoomModel game = CreateGameTest(4);
            Assert.AreNotEqual(null, game);
            Assert.AreEqual(false, string.IsNullOrEmpty(game.GameCode));
            Assert.AreEqual(4, game.GameCode.Length);
            Assert.AreEqual(null, ExitGameTest(game));
        }

        [Test]
        public void T_002_JoinFail()
        {
            Assert.AreEqual(null, JoinGameTest("****"));
        }

        [Test]
        public void T_003_ExitFail()
        {
            Assert.AreEqual(null, ExitGameTest(new GameRoomModel("****", Guid.NewGuid().ToString())));
        }

        [Test]
        public void T_004_JoinOverCap()
        {
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

        [Test]
        public void T_005_Start()
        {
            var game = CreateGameTest(3);
            Assert.AreEqual(3, game.MaxUserCount);
            Assert.AreEqual(1, game.UserCount);
            game = JoinGameTest(game.GameCode);
            Assert.AreEqual(2, game.UserCount);
            game = JoinGameTest(game.GameCode);
            Assert.AreEqual(3, game.UserCount);
            game = StartGameTest(game.GameCode);
            Assert.AreEqual(false, game.IsOpen);
            DeleteGameTest(game);
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

        private GameRoomModel StartGameTest(string gameCode)
        {
            return NetworkManager.StartGame(gameCode);
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
