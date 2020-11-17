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
    }

    private void Start()
    {
        handObject.Roll();
    }

    private void Init()
    {
        SpriteManager.Get();
        DataManager.Get();

        RoundCount = 1;

        mapObject.Init();
        mapObject.onFixPhaseExit += OnFixPhaseExit;
#if UNITY_EDITOR
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();
        mapObject.hand = handObject;

        stageData = JObject.Parse(stageJson.text).ToObject<StageModel>();
        handObject.stage = stageData;
#endif
        handObject.Init();

        onRoundCountChanged += OnRoundCountChanged;
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
}
