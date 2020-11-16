using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<int, RouteModel> RouteData { get; private set; }
    public Dictionary<int, DiceModel> DiceData { get; private set; }
    public Dictionary<int, StageListModel> StageListData { get; private set; }
    public Dictionary<int, MapModel> MapData { get; private set; }

    private void Awake()
    {
        RouteData = MakeDatabase<RouteModel>("Route");
        DiceData = MakeDatabase<DiceModel>("Dice");
        StageListData = MakeDatabase<StageListModel>("Stage");
        foreach (var stageItem in StageListData)
        {
            string path = $"Assets/Data/Stage/{stageItem.Value.Name}.json";
            var jsonTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            stageItem.Value.Stage = JObject.Parse(jsonTextAsset.text).ToObject<StageModel>();
        }
    }

    private Dictionary<int, T> MakeDatabase<T>(string dataname)
    {
        Dictionary<int, T> database = new Dictionary<int, T>();
        var json = JObject.Parse(File.ReadAllText($"{Application.dataPath}/Data/{dataname}.json"));
        var routeArray = json[dataname].ToArray();
        for (int i = 0; i < routeArray.Length; i++)
        {
            var route = routeArray[i].ToObject<T>();
            database.Add((route as IActor).Id, route);
        }

        return database;
    }
}
