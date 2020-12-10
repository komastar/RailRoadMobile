using Manager;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NetworkPlayTest
    {
        [UnityTest]
        public IEnumerator CRUDTest()
        {
            var code = NetworkManager.CreateGame(4);
            Assert.AreNotEqual(null, code);
            Assert.AreEqual(4, code.Length);
            Log.Info("Create SUC");
            //yield return new WaitForSeconds(1f);

            var r = NetworkManager.JoinGame(code);
            Assert.AreEqual(true, r);
            Log.Info("Join SUC");
            //yield return new WaitForSeconds(10f);

            r = NetworkManager.JoinGame(code);
            Assert.AreEqual(true, r);
            Log.Info("Join SUC 2");
            //yield return new WaitForSeconds(10f);

            r = NetworkManager.DeleteGame(code);
            Assert.AreEqual(true, r);
            Log.Info("Delete SUC");

            yield return null;
        }
    }
}
