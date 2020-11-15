using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameEditorWindow : EditorWindow
{
    public static MapObject mapObj;
    public static Dictionary<int, RouteModel> routeData;
    public static Dictionary<string, Sprite> spriteData;
    public static string[] routes;
    public static int selectRouteIndex;
    private readonly GUILayoutOption[] labelOption = new GUILayoutOption[] { GUILayout.Width(100) };
    private readonly GUILayoutOption[] popupOption = new GUILayoutOption[] { GUILayout.Width(300) };

    [MenuItem("DevAssist/Game Editor")]
    public static void OpenWindow()
    {
        EditorWindow window = GetWindow(typeof(GameEditorWindow));
        window.Show();
    }

    private void Awake()
    {
        Init();
        titleContent = new GUIContent("Game Editor");
        minSize = new Vector2(400, 400);
        maxSize = new Vector2(400, 1200);
    }

    private void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (routes != null)
                {
                    selectRouteIndex = EditorGUILayout.Popup("NodeType", selectRouteIndex, routes, GUILayout.Width(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
                    if (GUILayout.Button("Set"))
                    {
                        var route = routeData.Values.First(r => r.Name == routes[selectRouteIndex]);
                        mapObj.SetCandidate(route.Id);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (spriteData != null)
                {
                    string spriteName = routes[selectRouteIndex];
                    if (spriteData.ContainsKey(spriteName))
                    {
                        var sprite = spriteData[routes[selectRouteIndex]];
                        EditorGUILayout.ObjectField(sprite
                            , typeof(Texture2D)
                            , false
                            , GUILayout.Width(100)
                            , GUILayout.Height(100));
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                Init();
            }
        }
    }

    private void Init()
    {
        mapObj = FindObjectOfType<MapObject>();
        routeData = DataManager.Get().RouteData;
        spriteData = new Dictionary<string, Sprite>();

        routes = new string[routeData.Values.Count];
        int i = 0;
        foreach (var route in routeData)
        {
            routes[i++] = route.Value.Name;
            spriteData.Add(route.Value.Name, SpriteManager.Get().GetSprite(route.Value.Name));
        }
    }
}