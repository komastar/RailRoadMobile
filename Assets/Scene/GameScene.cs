using Manager;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private GameManager gameManager;
    private DataManager dataManager;

    public TextAsset mapJson;
    public TextAsset stageJson;
    public Text roundText;
    public Text chapterNameText;
    public Text stageNameText;
    public Text mapNameText;

    public MapObject mapObject;
    public HandObject handObject;
    public ScoreObject scoreObject;

    public ChapterModel currentChapter;
    public StageModel currentStage;

    private int roundCount;
    public int RoundCount
    {
        get => roundCount;
        set
        {
            if (value != roundCount)
            {
                roundCount = value;
            }

            onRoundCountChanged?.Invoke(roundCount);
        }
    }

    public Action<int> onRoundCountChanged;

    private void Awake()
    {
        Init();
        InitChapter();
        MakeStage();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetNextStage();
            MakeStage();
        }
    }

    private void Init()
    {
        SpriteManager.Get();
        gameManager = GameManager.Get();
        dataManager = DataManager.Get();

        if (ReferenceEquals(null, scoreObject))
        {
            scoreObject = FindObjectOfType<ScoreObject>();
        }
        scoreObject.Init();
        scoreObject.onClose += SetNextStage;
        scoreObject.onClose += MakeStage;

        if (ReferenceEquals(null, handObject))
        {
            handObject = FindObjectOfType<HandObject>();
        }
        handObject.Init();

        if (ReferenceEquals(null, mapObject))
        {
            mapObject = FindObjectOfType<MapObject>();
        }
        mapObject.hand = handObject;
        mapObject.Init();
        mapObject.onFixPhaseExit += OnFixPhaseExit;

        onRoundCountChanged = null;
        onRoundCountChanged += OnRoundCountChanged;
    }

    private void InitChapter()
    {
        currentChapter = dataManager.GetFirstChapter();
        currentStage = dataManager.GetFirstStage(currentChapter);
    }

    public void SetStage(StageModel stage)
    {
        currentStage = stage;
    }

    private void SetNextStage()
    {
        currentStage = dataManager.GetNextStage(currentChapter, currentStage);
        if (currentStage == null)
        {
            currentChapter = dataManager.GetNextChapter(currentChapter);
            currentStage = dataManager.GetFirstStage(currentChapter);
        }

        if (currentStage == null)
        {
            Application.Quit();
        }
    }

    public void MakeStage()
    {
        if (currentStage == null)
        {
            Log.Error("Stage is null");
            return;
        }

        chapterNameText.text = dataManager.Localize("Chapter", currentChapter?.Name);
        mapNameText.text = currentStage.MapName;
        stageNameText.text = currentStage.Name;

        handObject.stage = currentStage;
        handObject.Ready();

        mapJson = Resources.Load<TextAsset>($"Data/Map/{currentStage.MapName}");
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();

        handObject.Roll();

        RoundCount = 1;
    }

    public void OnClickCancel()
    {
        mapObject.CancelNode();
    }

    public void OnClickRotate()
    {
        mapObject.Rotate();
    }

    public void OnClickFlip()
    {
        mapObject.Flip();
    }

    public void OnClickFix()
    {
        OnFixPhaseExit();
    }

    public void OnClickRoll()
    {
        handObject.Roll();
    }

    public void OnFixPhaseExit()
    {
        if (mapObject.Fix())
        {
            if (RoundCount + 1 > currentStage.Round)
            {
                OnGameOver();
            }
            else
            {
                RoundCount++;
                mapObject.NewRound(RoundCount);
                handObject.Roll();
            }
        }
    }

    public void OnRoundCountChanged(int round)
    {
        roundText.text = $"Round : {round} / {currentStage?.Round}";
    }

    public void OnGameOver()
    {
        var score = mapObject.GetScore();
        gameManager.ReportScore(score);
        scoreObject.SetScore(score);
        scoreObject.Open();
    }
}
