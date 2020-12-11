using Assets.Foundation.Constant;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Object
{
    public class ContentRestrictObject : MonoBehaviour
    {
        private void Awake()
        {
            var response = NetworkManager.GetRequest($"{UrlTable.GameServer}/api/ApiContentLock/Check/{Application.productName}/{Application.version}/{name}");
            gameObject.SetActive(!response.ProcessResult);
        }
    }
}
