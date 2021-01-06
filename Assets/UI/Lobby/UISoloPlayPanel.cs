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
        public GameObject loadingPanel;

        private List<Button> stageButtonList;
        private Dictionary<int, UIStageButton> stageButtons;

        public Text chapterNameText;

        public Button chapterLeftButton;
        public Button chapterRightButton;

        public Transform chapterPages;
        public RectTransform chapterPagesRect;

        public UIChapterPage chapterPagePrefab;
        public List<UIChapterPage> chapterPageList;

        public float pagingSpeed;
        public int chapterIndex;
        public int chapterCount;
        public float chapterPageWidth;

        private Vector2 pageDestination;

        private void Update()
        {
            chapterPagesRect.anchoredPosition = Vector2.Lerp(chapterPagesRect.anchoredPosition, pageDestination, pagingSpeed * Time.deltaTime);
        }

        public override void Setup()
        {
            gameManager = GameManager.Get();
            dataManager = DataManager.Get();
            gameManager.GameRoom = GameRoomModel.GetSoloPlay();

            pageDestination = chapterPagesRect.anchoredPosition;
            chapterPageWidth = chapterPagePrefab.GetComponent<RectTransform>().rect.width;

            stageButtonList = new List<Button>();
            stageButtons = new Dictionary<int, UIStageButton>();
            chapterPageList = new List<UIChapterPage>();

            var chapters = dataManager.ChapterData;
            chapterIndex = gameManager.GetLastChapterIndex();
            chapterCount = chapters.Count;
            foreach (var chapter in chapters)
            {
                var chapterPage = Instantiate(chapterPagePrefab, chapterPages);
                chapterPage.SetName(chapter.Value.Name);
                chapterPageList.Add(chapterPage);
                int stageIndex = 0;
                foreach (var stage in chapter.Value.Stages)
                {
                    var stageButton = Instantiate(stageButtonPrefab, chapterPage.transform);                    
                    stageButton.StageData = dataManager.StageListData[stage].Stage;
                    stageButton.SetStageNumber(stageIndex + 1);
                    stageButton.SetStageStatus(false);
                    stageButtonList.Add(stageButton.ownButton);
                    stageButtons.Add(stage, stageButton);
                    stageIndex++;
                }
            }

            onBeforeOpen = null;
            onBeforeOpen += async () =>
            {
                UpdateChapterPage(true);

                bool isClear = true;
                foreach (var button in stageButtons)
                {
                    button.Value.ownButton.interactable = isClear;
                    isClear = gameManager.IsClearStage(button.Value.StageData.Id);
                    button.Value.SetStageStatus(isClear);
                }

                var clicked = await UIButtonAsync.SelectButton<UIStageButton>(stageButtonList.ToArray());
                Close();
                loadingPanel.SetActive(true);
                gameManager.CurrentChapter = dataManager.GetFirstChapter();
                gameManager.CurrentStage = clicked.StageData;
                gameManager.SetLastChapterIndex(chapterIndex);
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

        public void OnClickNextButton(bool isRight)
        {
            if (isRight)
            {
                chapterIndex++;
            }
            else
            {
                chapterIndex--;
            }

            UpdateChapterPage();
        }

        private void UpdateChapterPage(bool isInstant = false)
        {
            chapterIndex = Mathf.Clamp(chapterIndex, 0, chapterCount - 1);
            if (isInstant)
            {
                chapterPagesRect.anchoredPosition = new Vector3(-chapterPageWidth * chapterIndex, 0f);
                pageDestination = chapterPagesRect.anchoredPosition;
            }
            else
            {
                pageDestination = new Vector3(-chapterPageWidth * chapterIndex, 0f);
            }
            chapterNameText.text = chapterPageList[chapterIndex].Name;
            chapterLeftButton.interactable = chapterIndex > 0;
            chapterRightButton.interactable = chapterIndex < chapterCount - 1;
        }
    }
}