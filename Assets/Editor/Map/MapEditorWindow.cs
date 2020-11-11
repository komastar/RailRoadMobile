using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorWindow : EditorWindow
{
    public static string lastPath;

    public static MapObject map;
    public static MapModel mapData;
    public static int routeId;

    public static Dictionary<int, RouteModel> routeData;
    public static Dictionary<string, Sprite> spriteData;

    public static List<MapModel> mapList;

    readonly GUILayoutOption[] labelOption = new GUILayoutOption[] { GUILayout.Width(100) };

    private void Awake()
    {
        titleContent = new GUIContent("Map Editor");
        minSize = new Vector2(300, 400);
        maxSize = new Vector2(300, 1200);
        Refresh();
    }

    private void Refresh(bool isForce = false)
    {
        RefreshMapList(isForce);

        if (string.IsNullOrEmpty(lastPath) || isForce)
        {
            lastPath = Application.dataPath;
        }

        if (null == mapData || isForce)
        {
            mapData = new MapModel();
        }

        if (ReferenceEquals(null, map) || isForce)
        {
            map = FindObjectOfType<MapObject>();
        }

        if (null == routeData || isForce)
        {
            routeData = new Dictionary<int, RouteModel>();
            MakeDatabase("Route", ref routeData);
        }

        if (null == spriteData || isForce)
        {
            spriteData = new Dictionary<string, Sprite>();
            var sprites = AssetDatabase.LoadAllAssetsAtPath($"Assets/Sprites/RailRoadSprites.psd");
            for (int i = 0; i < sprites.Length; i++)
            {
                var sprite = sprites[i] as Sprite;
                if (sprite != null)
                {
                    spriteData.Add(sprites[i].name, sprite);
                }
            }
        }
    }

    private static void RefreshMapList(bool isForce)
    {
        if (null == mapList || isForce)
        {
            mapList = new List<MapModel>();
        }

        mapList.Clear();
        var maps = Directory.GetFiles($"{Application.dataPath}/Data/Map", "*.json");
        for (int i = 0; i < maps.Length; i++)
        {
            var mapJsonString = File.ReadAllText(maps[i]);
            var mapJsonObj = JObject.Parse(mapJsonString).ToObject<MapModel>();
            mapJsonObj.Filename = maps[i];
            mapList.Add(mapJsonObj);
        }
    }

    private void MakeDatabase<T>(string dataname, ref Dictionary<int, T> database)
    {
        database = new Dictionary<int, T>();
        var json = JObject.Parse(File.ReadAllText($"{Application.dataPath}/Data/{dataname}.json"));
        var routeArray = json[dataname].ToArray();
        for (int i = 0; i < routeArray.Length; i++)
        {
            var route = routeArray[i].ToObject<T>();
            database.Add((route as IActor).Id, route);
        }
    }

    [MenuItem("DevAssist/Map Editor")]
    public static void OpenWindow()
    {
        EditorWindow window = GetWindow(typeof(MapEditorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != "MapScene")
        {
            Close();

            return;
        }

        if (ReferenceEquals(null, map))
        {
            map = FindObjectOfType<MapObject>();
        }


        #region Map Data
        EditorGUILayout.BeginVertical();
        if (null != mapData)
        {
            EditorGUILayout.LabelField("Map Data", labelOption);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Map Name", labelOption);
                mapData.Name = EditorGUILayout.TextField(mapData.Name);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Map Size", labelOption);
                mapData.MapSize.x = EditorGUILayout.IntField(mapData.MapSize.x);
                mapData.MapSize.y = EditorGUILayout.IntField(mapData.MapSize.y);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Node Size", labelOption);
                mapData.NodeSize = EditorGUILayout.FloatField(mapData.NodeSize);
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("MAP DATA NOT LOADED");
        }
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.Space();

        #region Map Edit
        EditorGUILayout.BeginVertical();
        if (null!= mapData)
        {
            EditorGUILayout.LabelField("Map Mod");
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate"))
                {
                    map.MakeMap(mapData);
                }
                else if (GUILayout.Button("Clear"))
                {
                    map.Clear();
                }
                else if (GUILayout.Button("Save"))
                {
                    string path = EditorUtility.SaveFilePanel("MapData Save", lastPath, mapData.Name, "json");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    UpdateLastPath(path);
                    map.Save(path, mapData);
                }
                else if (GUILayout.Button("Load"))
                {
                    var path = EditorUtility.OpenFilePanel("Open Map Data", $"{Application.dataPath}/Data/.Map", "json");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    UpdateLastPath(path);
                    EditorUtility.DisplayProgressBar("Load Map Data", "Make Database", .3f);
                    Refresh(true);
                    var jsonString = File.ReadAllText(path);
                    mapData = JObject.Parse(jsonString).ToObject<MapModel>();
                    EditorUtility.DisplayProgressBar("Load Map Data", "Make Nodes", .6f);
                    map.MakeMap(mapData, routeData, spriteData);
                    EditorUtility.DisplayProgressBar("Load Map Data", "Done", 1f);
                    EditorUtility.ClearProgressBar();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.Space();

        #region Node Edit
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Node Mod");
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Rotate"))
                {
                    var nodes = GetAllSelected();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (!ReferenceEquals(null, nodes[i]))
                        {
                            nodes[i].Rotate();
                        }
                    }
                }
                else if (GUILayout.Button("Reset"))
                {
                    var nodes = GetAllSelected();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (!ReferenceEquals(null, nodes[i]))
                        {
                            nodes[i].ResetNode();
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                routeId = EditorGUILayout.IntField(routeId);
                if (GUILayout.Button("Set"))
                {
                    SetNode(routeId);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Rail"))
                {
                    SetNode(1001);
                }
                else if (GUILayout.Button("Road"))
                {
                    SetNode(1002);
                }
                else if (GUILayout.Button("Wall"))
                {
                    SetNode(1000);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        {
            if (GUILayout.Button("Refresh"))
            {
                Refresh(true);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (null != mapList && mapList.Count > 0)
        {
            for (int i = 0; i < mapList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(Path.GetFileName(mapList[i].Filename), labelOption);
                    EditorGUILayout.LabelField(mapList[i].Name, labelOption);
                    if (GUILayout.Button("Open"))
                    {
                        UpdateLastPath(mapList[i].Filename);
                        mapData = mapList[i];
                        map.MakeMap(mapList[i], routeData, spriteData);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    private static void UpdateLastPath(string path)
    {
        lastPath = Path.GetDirectoryName(path);
    }

    private void SetNode(int id)
    {
        string spriteName = routeData[id].Name;
        var nodes = GetAllSelected();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!ReferenceEquals(null, nodes[i]))
            {
                Sprite sprite = null;
                if (spriteData.ContainsKey(spriteName))
                {
                    sprite = spriteData[spriteName];
                }
                nodes[i].SetupNode(id, sprite);
            }
        }
    }

    private List<NodeObject> GetAllSelected()
    {
        var objs = Selection.gameObjects;
        List<NodeObject> nodes = new List<NodeObject>();
        for (int i = 0; i < objs.Length; i++)
        {
            nodes.Add(objs[i].GetComponent<NodeObject>());
        }

        return nodes;
    }
}
