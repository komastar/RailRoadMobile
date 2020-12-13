using Assets.Foundation.Constant;
using Manager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Tests.EditMode
{
    public class GameUserTest
    {
        [Test]
        public void T_001_Create()
        {
            var userId = NetworkManager.CreateUser();
            Assert.AreNotEqual(true, string.IsNullOrEmpty(userId));
            Log.Info(userId);
        }

        [Test]
        public void T_999_Clear()
        {
            var result = NetworkManager.ClearUser();
            Assert.Greater(result, 0);
        }
    }
}
