using Assets.Foundation.Constant;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scene
{
    public class TestScene : MonoBehaviour
    {
        private NetworkManager netManager;

        private void Awake()
        {
            netManager = NetworkManager.Get();
        }

        private void Start()
        {
            netManager.GetRequest(UrlTable.GetCreateGameUrl(2));
        }
    }
}
