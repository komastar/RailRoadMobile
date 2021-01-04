using Assets.Foundation.Model;
using Assets.Foundation.UI.Common;
using Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UISoloPlayPanel : UIPanel
    {
        private GameManager gameManager;
        private DataManager dataManager;

        public UIStageButton stageButtonPrefab;
        public Transform stagePanel;
        public GameObject loadingPanel;

        private Dictionary<int, UIStageButton> stageButtons;

        public override void Setup()
        {
            gameManager = GameManager.Get();
            dataManager = DataManager.Get();
            stageButtons = new Dictionary<int, UIStageButton>();
            gameManager.GameRoom = GameRoomModel.GetSoloPlay();

            var stages = dataManager.StageListData;
            buttons = new Button[stages.Count];
            int buttonIndex = 0;
            foreach (var stage in stages)
            {
                var button = Instantiate(stageButtonPrefab, stagePanel);
                button.StageData = stage.Value.Stage;
                button.SetStageNumber(buttonIndex + 1);
                button.SetStageStatus(false);
                buttons[buttonIndex] = button.ownButton;
                stageButtons.Add(stage.Value.Id, button);
                buttonIndex++;
            }

            onBeforeOpen = null;
            onBeforeOpen += async () =>
            {
                bool isClear = true;
                foreach (var button in stageButtons)
                {
                    button.Value.ownButton.interactable = isClear;
                    isClear = gameManager.IsClearStage(button.Value.StageData.Id);
                    button.Value.SetStageStatus(isClear);
                }

                var clicked = await UIButtonAsync.SelectButton<UIStageButton>(buttons);
                Close();
                loadingPanel.SetActive(true);
                gameManager.CurrentChapter = dataManager.GetFirstChapter();
                gameManager.CurrentStage = clicked.StageData;
                var loadScene = SceneManager.LoadSceneAsync("GameScene");
                loadScene.completed += (op) =>
                {
                    loadScene.allowSceneActivation = true;
                };
            };
        }

        public override void Close()
        {
            base.Close();
        }
    }
}