using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainGameScene : MonoBehaviour
{
#if UNITY_EDITOR
    public TextAsset mapJson;
#endif
    public MapObject mapObject;

    private void Awake()
    {
        SpriteManager.Get();
        DataManager.Get();

        mapObject.Init();
#if UNITY_EDITOR
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();
#endif
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
}
