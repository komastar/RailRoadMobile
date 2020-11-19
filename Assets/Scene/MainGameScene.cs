using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainGameScene : MonoBehaviour
{
#if UNITY_EDITOR
    public TextAsset mapJson;
    public TextAsset stageJson;
#endif
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

    private void MakeStage()
    {
        RoundCount = 1;

        mapObject.hand = handObject;
#if UNITY_EDITOR
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();

        stageData = JObject.Parse(stageJson.text).ToObject<StageModel>();
        handObject.stage = stageData;
#endif

        handObject.Roll();
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
        roundText.text = round.ToString();
    }

    public void OnGameOver()
    {
        int score = mapObject.GetScore();
        scoreObject.Open();
        scoreObject.SetScore(score);
    }
}
