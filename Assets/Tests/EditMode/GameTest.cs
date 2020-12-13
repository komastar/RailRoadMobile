using System.Threading;
using Assets.Foundation.Constant;
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
            var gameCode = CreateGameTest(4);
            Assert.AreNotEqual(null, gameCode);
            Assert.AreEqual(4, gameCode.Length);
            Assert.AreEqual(true, JoinGameTest(gameCode));
            Assert.AreEqual(true, DeleteGameTest(gameCode));
        }

        [Test]
        public void T_002_JoinFail()
        {
            Assert.AreEqual(false, JoinGameTest("****"));
        }

        [Test]
        public void T_003_DeleteFail()
        {
            Assert.AreEqual(false, DeleteGameTest("****"));
        }

        [Test]
        public void T_004_JoinOverCap()
        {
            var code = CreateGameTest(4);
            Assert.AreNotEqual(null, code);
            Assert.AreEqual(4, code.Length);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(true, JoinGameTest(code));
            }
            Assert.AreEqual(false, JoinGameTest(code));
            Assert.AreEqual(true, DeleteGameTest(code));
        }

        [Test]
        public void T_005_Ready()
        {
            UrlTable.IsRemote = false;
            var code = CreateGameTest(2);
            Log.Info("Ready begin 1");
            NetworkManager.ReadyGame(code, 1000);
            Log.Info("Ready end 1");
            Log.Info("Ready begin 2");
            NetworkManager.ReadyGame(code, 5000);
            Log.Info("Ready end 2");
        }

        private string CreateGameTest(int userCount)
        {
            Log.Info($"CREATE {userCount}");
            return NetworkManager.CreateGame(userCount);
        }

        private bool JoinGameTest(string gameCode)
        {
            Log.Info($"JOIN {gameCode}");
            return NetworkManager.JoinGame(gameCode);
        }

        private bool DeleteGameTest(string gameCode)
        {
            Log.Info($"DELETE {gameCode}");
            return NetworkManager.DeleteGame(gameCode);
        }
    }
}
