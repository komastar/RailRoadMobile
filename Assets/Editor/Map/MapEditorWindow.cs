using Assets.Manager;
using Assets.Map.MapEditor;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorWindow : EditorWindow
{
    public Map map;

    public static string mapName = "NewMap";
    public static float nodeSize = 1f;
    public static GridInt mapSize = new GridInt(5, 5);
    public static int routeId;

    public static Dictionary<int, RouteModel> routeData;
    public static Dictionary<string, Sprite> spriteData;

    private void Awake()
    {
        Refresh();
    }

    private void Refresh(bool isForce = false)
    {
        if (ReferenceEquals(null, map) || isForce)
        {
            map = FindObjectOfType<Map>();
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

    private void MakeDatabase<T>(string dataname, ref Dictionary<int, T> database)
    {
        database = new Dictionary<int, T>();
        var json = JObject.Parse(File.ReadAllText($"{Application.dataPath}/Data/{dataname}.json"));
        var routeArray = json[dataname].ToArray();
        for (int i = 0; i < routeArray.Length; i++)
        {
            var route = routeArray[i].ToObject<T>();
            database.Add((route as IDataModel).Id, route);
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
            FindObjectOfType<Map>();
        }

        #region Map Data
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Map Name");
                mapName = EditorGUILayout.TextField(mapName);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Map Size");
                mapSize.X = EditorGUILayout.IntField(mapSize.X);
                mapSize.Y = EditorGUILayout.IntField(mapSize.Y);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Node Size");
                nodeSize = EditorGUILayout.FloatField(nodeSize);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.Space();

        #region Map Edit
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Map Mod");
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate"))
                {
                    map.mapData = new MapModel
                    {
                        Name = mapName,
                        MapSize = mapSize,
                        NodeSize = nodeSize
                    };
                    map.Clear();
                    map.Generate();
                }
                else if (GUILayout.Button("Clear"))
                {
                    map.Clear();
                }
                else if (GUILayout.Button("Save"))
                {
                    map.mapData = new MapModel
                    {
                        Name = mapName,
                        MapSize = mapSize,
                        NodeSize = nodeSize
                    };
                    map.Save();
                }
                else if (GUILayout.Button("Load"))
                {
                    var path = EditorUtility.OpenFilePanel("Open Map Data", $"{Application.dataPath}/Data/.Map", "json");
                    EditorUtility.DisplayProgressBar("Load Map Data", "Make Database", .3f);
                    Refresh(true);
                    var jsonString = File.ReadAllText(path);
                    map.mapData = JObject.Parse(jsonString).ToObject<MapModel>();
                    map.Clear();
                    EditorUtility.DisplayProgressBar("Load Map Data", "Make Nodes", .6f);
                    map.Load(routeData, spriteData);
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
                    map.Rotate();
                }
                else if (GUILayout.Button("Reset"))
                {
                    map.Reset();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                routeId = EditorGUILayout.IntField(routeId);
                if (GUILayout.Button("Set"))
                {
                    string spriteName = routeData[routeId].Name;
                    map.SetRoute(routeId, spriteData[spriteName]);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Rail"))
                {
                    routeId = 1001;
                }
                else if (GUILayout.Button("Road"))
                {
                    routeId = 1002;
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
    }
}
