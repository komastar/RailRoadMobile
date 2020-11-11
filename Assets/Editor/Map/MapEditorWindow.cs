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
    public static string lastSavePath;

    public static MapObject map;
    public static MapModel mapData;
    public static int routeId;

    public static Dictionary<int, RouteModel> routeData;
    public static Dictionary<string, Sprite> spriteData;

    private void Awake()
    {
        Refresh();
    }

    private void Refresh(bool isForce = false)
    {
        if (string.IsNullOrEmpty(lastSavePath) || isForce)
        {
            lastSavePath = Application.dataPath;
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
        if (null != mapData)
        {
            EditorGUILayout.LabelField("Map Data");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Map Name");
                mapData.Name = EditorGUILayout.TextField(mapData.Name);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Map Size");
                mapData.MapSize.x = EditorGUILayout.IntField(mapData.MapSize.x);
                mapData.MapSize.y = EditorGUILayout.IntField(mapData.MapSize.y);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Node Size");
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
                    map.Clear();
                    map.MakeMap(mapData);
                }
                else if (GUILayout.Button("Clear"))
                {
                    map.Clear();
                }
                else if (GUILayout.Button("Save"))
                {
                    string path = EditorUtility.SaveFilePanel("MapData Save", lastSavePath, mapData.Name, "json");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    lastSavePath = path;
                    map.Save(lastSavePath, mapData);
                }
                else if (GUILayout.Button("Load"))
                {
                    var path = EditorUtility.OpenFilePanel("Open Map Data", $"{Application.dataPath}/Data/.Map", "json");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }
                    EditorUtility.DisplayProgressBar("Load Map Data", "Make Database", .3f);
                    Refresh(true);
                    var jsonString = File.ReadAllText(path);
                    mapData = JObject.Parse(jsonString).ToObject<MapModel>();
                    map.Clear();
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
                    string spriteName = routeData[routeId].Name;
                    var nodes = GetAllSelected();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (!ReferenceEquals(null, nodes[i]))
                        {
                            nodes[i].SetupNode(routeId, spriteData[spriteName]);
                        }
                    }
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

    private List<NodeObject> GetAllSelected()
    {
        var objs = Selection.objects;
        List<NodeObject> nodes = new List<NodeObject>();
        for (int i = 0; i < objs.Length; i++)
        {
            nodes.Add((objs[i] as GameObject).GetComponent<NodeObject>());
        }

        return nodes;
    }
}
