using System.Collections;
using UnityEngine;

namespace Komastar.UI.Common
{
    public class UIRootPanel : MonoBehaviour
    {
        public Vector2 Ratio;
        public RectTransform[] leftRightPanel;
        public RectTransform[] topBotPanel;

        private void Awake()
        {
            float refRatio = Ratio.x / Ratio.y;
            float ratio = Screen.height / (float)Screen.width;
            float diffRatio = refRatio - ratio;
            float camRatio = (refRatio / ratio);
            Vector2 sizeDelta = Vector2.zero;
            Rect camRect = Camera.main.rect;
            if (0f > diffRatio)
            {
                //  9:21
                float elem = Screen.width / 9f;
                float sizeOverride = elem * 16f;
                sizeDelta.y = sizeOverride - Screen.height;
                for (int i = 0; i < topBotPanel.Length; i++)
                {
                    topBotPanel[i].sizeDelta = sizeDelta * -0.5f;
                }

                camRect.height = camRatio;
                camRect.y = (1f - camRect.height) * 0.5f;
            }
            else if (0f < diffRatio)
            {
                //  10:16
                float elem = Screen.height / 16f;
                float sizeOverride = elem * 9f;
                sizeDelta.x = sizeOverride - Screen.width;
                for (int i = 0; i < leftRightPanel.Length; i++)
                {
                    leftRightPanel[i].sizeDelta = sizeDelta * -0.5f;
                }

                camRect.width = camRect.width / camRatio;
                camRect.x = (1f - camRect.width) * 0.5f;
            }

            Camera.main.rect = camRect;
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = sizeDelta;
        }
    }
}