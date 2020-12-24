using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UIPanel : MonoBehaviour, IPointerClickHandler
    {
        public Button[] buttons;

        public Action onBeforeOpen;
        public Action onAfterClose;

        private void Update()
        {
            if (true == Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public virtual void Setup() { }

        public void Open()
        {
            onBeforeOpen?.Invoke();
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            onAfterClose?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Close();
        }
    }
}