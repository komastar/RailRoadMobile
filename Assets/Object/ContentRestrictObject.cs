using UnityEngine;
#if UNITY_EDITOR
#else
using Manager;
using Assets.Foundation.Constant;
#endif

namespace Assets.Object
{
    public class ContentRestrictObject : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            gameObject.SetActive(false);
#else
            NetworkManager.Get().GetRequest(UrlTable.GetContentLockCheckUrl(Application.productName, Application.version, name)
                , (response) =>
                {
                    bool isLock = bool.Parse(response);
                    gameObject.SetActive(!isLock);
                });
#endif
        }
    }
}
