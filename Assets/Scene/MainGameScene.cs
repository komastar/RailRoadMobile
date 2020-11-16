using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainGameScene : MonoBehaviour
{
#if UNITY_EDITOR
    public TextAsset mapJson;
    public TextAsset stageJson;
#endif
    public MapObject mapObject;
    public HandObject handObject;

    private void Awake()
    {
        SpriteManager.Get();
        DataManager.Get();

        mapObject.Init();
#if UNITY_EDITOR
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();
        mapObject.hand = handObject;

        handObject.stage = JObject.Parse(stageJson.text).ToObject<StageModel>();
#endif
        handObject.Init();
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
}
