using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainGameScene : MonoBehaviour
{
    public TextAsset mapJson;
    public MapObject mapObject;

    private StageController stage;
    private DiceController dice;

    private HashSet<NodeObject> openNodes;

    private void Awake()
    {
        SpriteManager.Get();
        DataManager.Get();

        openNodes = new HashSet<NodeObject>();

        stage = new StageController();
        dice = new DiceController();

        mapObject.Init();

#if UNITY_EDITOR
        MapModel map = JObject.Parse(mapJson.text).ToObject<MapModel>();
        mapObject.MakeMap(map);
        mapObject.OpenMap();
#endif
    }
}
