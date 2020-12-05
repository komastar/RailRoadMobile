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
    }
}
