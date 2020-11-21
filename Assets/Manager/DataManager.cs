using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
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
            string path = $"Data/Stage/{stageItem.Value.Name}";
            var jsonTextAsset = Resources.Load<TextAsset>(path);
            stageItem.Value.Stage = JObject.Parse(jsonTextAsset.text).ToObject<StageModel>();
        }
    }

    private Dictionary<int, T> MakeDatabase<T>(string dataname)
    {
        Dictionary<int, T> database = new Dictionary<int, T>();
        var textAsset = Resources.Load<TextAsset>($"Data/{dataname}");
        var json = JObject.Parse(textAsset.text);
        var routeArray = json[dataname].ToArray();
        for (int i = 0; i < routeArray.Length; i++)
        {
            var route = routeArray[i].ToObject<T>();
            database.Add((route as IActor).Id, route);
        }

        return database;
    }
}
