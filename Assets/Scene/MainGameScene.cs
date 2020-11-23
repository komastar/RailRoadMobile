using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainGameScene : MonoBehaviour
{
    public TextAsset mapJson;
    public TextAsset stageJson;
    public Text roundText;

    public MapObject mapObject;
    public HandObject handObject;
    public ScoreObject scoreObject;
    public StageModel stageData;

    private int roundCount;
    public int RoundCount
    {
        get => roundCount;
        set
        {
            if (value != roundCount)
            {
                roundCount = value;
                onRoundCountChanged?.Invoke(roundCount);
            }
        }
    }

    public Action<int> onRoundCountChanged;

    private void Awake()
    {
        Init();
        MakeStage();
    }

    private void Init()
    {
        SpriteManager.Get();
        DataManager.Get();

        if (ReferenceEquals(null, scoreObject))
        {
            scoreObject = FindObjectOfType<ScoreObject>();
        }
        scoreObject.Init();
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
        mapObject.Init();
        mapObject.onFixPhaseExit += OnFixPhaseExit;

        onRoundCountChanged = null;
        onRoundCountChanged += OnRoundCountChanged;
    }

    public void MakeStage()
    {
        mapObject.hand = handObject;
        stageData = JObject.Parse(stageJson.text).ToObject<StageModel>();
        handObject.stage = stageData;

        mapJson = Resources.Load<TextAsset>($"Data/Map/{stageData.MapName}");
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();

        handObject.Roll();

        RoundCount = 1;
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
        mapObject.FixNode();
    }

    public void OnClickRoll()
    {
        handObject.Roll();
    }

    public void OnFixPhaseExit()
    {
        if (0 == handObject.GetDiceCount())
        {
            if (RoundCount + 1 > stageData.Round)
            {
                mapObject.Close();
                OnGameOver();
            }
            else
            {
                RoundCount++;
                handObject.Roll();
            }
        }
    }

    public void OnRoundCountChanged(int round)
    {
        roundText.text = $"Round : {round} / {stageData?.Round}";
    }

    public void OnGameOver()
    {
        var score = mapObject.GetScore();
        scoreObject.SetScore(score);
        scoreObject.Open();
    }
}
