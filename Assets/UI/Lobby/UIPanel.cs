using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UIPanel : MonoBehaviour
    {
        public Button[] buttons;

        private void Update()
        {
            if (true == Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}