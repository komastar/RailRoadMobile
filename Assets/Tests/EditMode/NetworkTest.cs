using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Manager;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NetworkTest
    {
        [Test]
        public void AsyncSocketTest()
        {
            Log.Info("Socket test start");
            NetworkManager.StartClient();
            Log.Info("Socket test end");
        }

        [Test]
        public async void AsyncWebSocketTest()
        {
            await NetworkManager.StartWebSocket();
        }

        static string gameCode = "";

        [Test]
        public void CreateGameTest()
        {
            Log.Info($"CREATE 4 user");
            gameCode = NetworkManager.CreateGame(4);
            Assert.AreNotEqual(null, gameCode);
            Assert.AreEqual(4, gameCode.Length);
            Log.Info(gameCode);
            Log.Info("Create SUC PASS");
        }

        [Test]
        public void JoinGameTest()
        {
            Log.Info($"JOIN TO : {gameCode}");
            var isJoin = NetworkManager.JoinGame(gameCode);
            Assert.AreEqual(true, isJoin);
            Log.Info("Join SUC PASS");
            Thread.Sleep(100);
        }

        [Test]
        public void DeleteGameTest()
        {
            Log.Info($"DELETE {gameCode}");
            var isDelete = NetworkManager.DeleteGame(gameCode);
            Assert.AreEqual(true, isDelete);
            Log.Info("Delete SUC PASS");
        }

        [Test]
        public void CRUDGameTest()
        {
            CreateGameTest();
            JoinGameTest();
            DeleteGameTest();
        }
    }
}
