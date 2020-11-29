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
    public Dictionary<int, ChapterModel> ChapterData { get; private set; }
    public JObject LocalizeData { get; private set; }

    private void Awake()
    {
        RouteData = MakeDatabase<RouteModel>("Route");
        DiceData = MakeDatabase<DiceModel>("Dice");
        ChapterData = MakeDatabase<ChapterModel>();
        StageListData = MakeDatabase<StageListModel>("Stage");
        foreach (var stageItem in StageListData)
        {
            string path = $"Data/Stage/{stageItem.Value.Name}";
            var jsonTextAsset = Resources.Load<TextAsset>(path);
            var stage = JObject.Parse(jsonTextAsset.text).ToObject<StageModel>();
            stage.Id = stageItem.Key;
            stageItem.Value.Stage = stage;
        }

        var koLocalizeData = Resources.Load<TextAsset>("Data/Localization/ko-kr");
        LocalizeData = JObject.Parse(koLocalizeData.text);
    }

    private Dictionary<int, T> MakeDatabase<T>(string dataname = "")
    {
        if (string.IsNullOrEmpty(dataname))
        {
            var t = typeof(T);
            dataname = t.Name.Replace("Model", "");
        }
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

    public ChapterModel GetFirstChapter()
    {
        return ChapterData.Values.First();
    }

    public StageModel GetFirstStage(ChapterModel chapter)
    {
        if (null == chapter)
        {
            return null;
        }

        int firstStageId = chapter.Stages.First();
        return StageListData[firstStageId].Stage;
    }

    public ChapterModel GetNextChapter(ChapterModel chapter)
    {
        return ChapterData[chapter.NextChapterId];
    }

    public StageModel GetNextStage(ChapterModel chapter, StageModel stage)
    {
        if (chapter.Stages.Contains(stage.Id))
        {
            int stageIndex = 0;
            for (int i = 0; i < chapter.Stages.Length; i++)
            {
                stageIndex++;
                if (stage.Id == chapter.Stages[i])
                {
                    break;
                }
            }

            if (stageIndex < chapter.Stages.Length)
            {
                int nextStageId = chapter.Stages[stageIndex];
                return StageListData[nextStageId].Stage;
            }
        }

        return null;
    }

    public string Localize(string category, string name)
    {
        string key = $"{category}.{name}";
        var t = LocalizeData.SelectToken(key);
        if (null != t)
        {
            return t.ToString();
        }
        else
        {
            return key;
        }
    }
}
