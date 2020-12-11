using Assets.Foundation.Constant;
using Manager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.RESTAPI.ContentLock
{
    public class ContentLockTest
    {
        private static string url = $"{UrlTable.GameServerRemote}/api/ApiContentLock/Check/TestProject";
        [Test]
        public void T_001_Lock()
        {
            Request("1.0.0", "LockedUp", false);
        }

        [Test]
        public void T_002_Unlock()
        {
            Request("1.0.0", "Unlock", true);
        }

        [Test]
        public void T_011_VersionLock()
        {
            Request("0.1.0", "Version1", false);
        }

        [Test]
        public void T_012_VersionUnlock()
        {
            Request("1.0.0", "Version1", true);
        }

        private void Request(string version, string name, bool expected)
        {
            Log.Info($"{version} / {name} / {expected}");
            var response = NetworkManager.GetRequest($"{url}/{version}/{name}");
            Assert.AreEqual(expected, response.ProcessResult);
        }
    }
}
