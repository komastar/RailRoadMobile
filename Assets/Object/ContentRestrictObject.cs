using UnityEngine;

namespace Assets.Object
{
    public class ContentRestrictObject : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            gameObject.SetActive(false);
#else
            var response = NetworkManager.GetRequest($"{UrlTable.GameServer}/api/ApiContentLock/Check/{Application.productName}/{Application.version}/{name}");
            gameObject.SetActive(!response.ProcessResult);
#endif
        }
    }
}
