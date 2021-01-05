using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Foundation.UI.Common
{
    public class UICllickClose : MonoBehaviour, IPointerClickHandler
    {
        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}