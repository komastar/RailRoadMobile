using Assets.Foundation.Constant;
using Assets.Object;
using Manager;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
    public Text timerText;
    public Text gameCodeText;

    public MapObject mapObject;
    public HandObject handObject;
    public ScoreObject scoreObject;
    public GameRoomObject gameRoomObject;

    public ChapterModel currentChapter;
    public StageModel currentStage;
    public ScoreViewModel score;

    private Coroutine timerCoroutine;

    [SerializeField]
    private int timerCount;
    public int TimerCount
    {
        get => timerCount;
        set
        {
            timerCount = value;
            timerText.text = timerCount.ToString();
        }
    }

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
    private Action onTimeOver;

    private void Awake()
    {
        gameRoomObject.Open();
        Init();
        gameRoomObject.SetGameRoom(gameManager.GameRoom);
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
        score = new ScoreViewModel();

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

        onTimeOver += OnClickFix;

        gameRoomObject.onDisable += StartGame;
    }

    private void StartGame()
    {
        InitChapter();
        MakeStage();
    }

    private void InitChapter()
    {
        if (GameCode.SoloPlay == gameManager.GameRoom.GameCode)
        {
            currentChapter = dataManager.GetFirstChapter();
            currentStage = dataManager.GetFirstStage(currentChapter);
        }
        else
        {
            currentChapter = dataManager.GetPvpChapter();
            currentStage = dataManager.GetPvpStage(currentChapter);
        }

        gameCodeText.text = gameManager.GameRoom.GameCode;
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

        RoundCount = 0;
        GoNextRound();
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
        int contructFailCount = mapObject.Fix();
        score.ConstructFailScore += contructFailCount;
        if (RoundCount + 1 > currentStage.Round)
        {
            OnGameOver();
        }
        else
        {
            GoNextRound();
        }
    }

    private void GoNextRound()
    {
        if (GameCode.SoloPlay != gameManager.GameRoom.GameCode)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Round/{gameManager.GameRoom.GameCode}/{RoundCount}";
            StartCoroutine(NetworkManager.GetRequestAsync(url, OnRoundComplete));
            timerText.text = "Waiting...";
        }
        if (null != timerCoroutine)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    private void OnRoundComplete(string p)
    {
        RoundCount++;
        mapObject.NewRound(RoundCount);
        handObject.Roll();

        timerCoroutine = StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        TimerCount = currentStage.TimePerRound == 0 ? NumTable.DefaultRoundTime : currentStage.TimePerRound;
        while (0 < TimerCount)
        {
            yield return new WaitForSeconds(1f);

            TimerCount--;
        }

        onTimeOver?.Invoke();
    }

    public void OnRoundCountChanged(int round)
    {
        roundText.text = $"Round : {round} / {currentStage?.Round}";
    }

    public void OnGameOver()
    {
        TimerCount = 0;
        mapObject.GetScore(score);
        gameManager.ReportScore(score);
        scoreObject.SetScore(score);
        scoreObject.Open();
    }
}
