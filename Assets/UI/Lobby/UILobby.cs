using Assets.Foundation.UI.Common;
using Assets.Foundation.UI.PopUp;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UILobby : MonoBehaviour
    {
        public UIPopUpPanel popUpPanel;

        public Button[] lobbyButtons;

        public UISoloPlayPanel uiSoloPlayPanel;
        public UIMultiPlayPanel uiMultiPlayPanel;
        public UIManualPanel uiManualPanel;

        private Stack<UIPanel> panelOpenStack;

        private void Awake()
        {
            Manager.GameManager.Get();
            panelOpenStack = new Stack<UIPanel>();
            uiSoloPlayPanel.Setup();
            uiMultiPlayPanel.Setup();
            uiManualPanel.Setup();
        }

        private async void Start()
        {
            while (isActiveAndEnabled)
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
                else if ("ManualButton" == select.name)
                {
                    OpenPanel(uiManualPanel);
                }

                await Task.Yield();
            }
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (0 == panelOpenStack.Count)
                {
                    await ExitGame();
                }
                else
                {
                    panelOpenStack.Pop().Close();
                }
            }
        }

        private void OpenPanel(UIPanel panel)
        {
            panel.Open();
            panelOpenStack.Push(panel);
            panel.onAfterClose = null;
            panel.onAfterClose += () =>
            {
                if (0 < panelOpenStack.Count
                && panelOpenStack.Peek() == panel)
                {
                    panelOpenStack.Pop();
                }
            };
        }
        private async Task ExitGame()
        {
            var confirm = (UIConfirmPopUp)popUpPanel.Open("Confirm");
            confirm.titleText.text = "게임을 종료하시겠습니까?";
            confirm.PositiveText.text = "예";
            confirm.NegativeText.text = "아니오";
            var result = await confirm.GetResult();
            if (true == result)
            {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#elif UNITY_ANDROID
            Application.Quit();
#endif
            }
            else
            {
                popUpPanel.TurnOff();
            }
        }
    }
}
