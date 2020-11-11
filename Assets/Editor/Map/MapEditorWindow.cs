using Assets.Map.MapEditor;
using System;
using System.Collections.Generic;
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

    private void Awake()
    {
        map = FindObjectOfType<Map>();
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
                    map.Save();
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
                    map.SetRoute(routeId);
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
    }
}
