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
            UrlTable.IsRemote = true;
            var game = CreateGameTest(4);
            Assert.AreNotEqual(null, game);
            Assert.AreNotEqual(null, game.GameCode);
            Assert.AreEqual(4, game.GameCode.Length);
            Assert.AreNotEqual(null, JoinGameTest(game.GameCode));
            Assert.AreEqual(null, ExitGameTest(game.GameCode));
        }

        [Test]
        public void T_002_JoinFail()
        {
            Assert.AreEqual(null, JoinGameTest("****"));
        }

        [Test]
        public void T_003_ExitFail()
        {
            Assert.AreEqual(null, ExitGameTest("****"));
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
            Assert.AreNotEqual(null, ExitGameTest(game.GameCode));
        }

        [Test]
        public void T_005_Ready()
        {
            var game = CreateGameTest(2);
            Log.Info("Ready begin 1");
            NetworkManager.ReadyGame(game.GameCode, 1000);
            Log.Info("Ready end 1");
            Log.Info("Ready begin 2");
            NetworkManager.ReadyGame(game.GameCode, 5000);
            Log.Info("Ready end 2");
        }

        private GameModel CreateGameTest(int userCount)
        {
            Log.Info($"CREATE {userCount}");
            return NetworkManager.CreateGame(userCount);
        }

        private GameModel JoinGameTest(string gameCode)
        {
            Log.Info($"JOIN {gameCode}");
            return NetworkManager.JoinGame(gameCode);
        }

        private GameModel ExitGameTest(string gameCode)
        {
            Log.Info($"DELETE {gameCode}");
            return NetworkManager.ExitGame(gameCode);
        }
    }
}
