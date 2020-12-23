using System.Collections.Generic;
using System.Linq;
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
        }

        private async void Start()
        {
            while (true)
            {
                Button select = await SelectButton();
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

        private async Task<Button> SelectButton()
        {
            var tasks = lobbyButtons.Select(PressButton);
            Task<Button> finish = await Task.WhenAny(tasks);

            return finish.Result;
        }

        private async Task<Button> PressButton(Button button)
        {
            bool isPressed = false;
            button.onClick.AddListener(() => isPressed = true);
            while (!isPressed)
            {
                await Task.Yield();
            }

            return button;
        }
    }
}
