using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<int, RouteModel> RouteData { get; private set; }
    public Dictionary<int, DiceModel> DiceData { get; private set; }

    private void Awake()
    {
        RouteData = MakeDatabase<RouteModel>("Route");
        DiceData = MakeDatabase<DiceModel>("Dice");
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
