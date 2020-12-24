using Assets.UI.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UILobby : MonoBehaviour
    {
        public Button[] lobbyButtons;

        public UISoloPlayPanel uiSoloPlayPanel;
        public UIMultiPlayPanel uiMultiPlayPanel;

        private Stack<UIPanel> panelOpenStack;

        private void Awake()
        {
            panelOpenStack = new Stack<UIPanel>();
            uiSoloPlayPanel.Setup();
            uiMultiPlayPanel.Setup();
        }

        private async void Start()
        {
            while (true)
            {
                Button select = await UIButtonAsync.SelectButton<Button>(lobbyButtons);
                if ("SoloPlayButton" == select.name)
                {
                    OpenPanel(uiSoloPlayPanel);
                }
                else if ("MultiPlayButton" == select.name)
                {
                    OpenPanel(uiMultiPlayPanel);
                }

                await Task.Yield();
            }
        }

        private void OpenPanel(UIPanel panel)
        {
            if (0 < panelOpenStack.Count)
            {
                UIPanel recentOpenPanel = panelOpenStack.Pop();
                recentOpenPanel.Close();
            }
            panel.Open();
            panelOpenStack.Push(panel);
        }
    }
}
