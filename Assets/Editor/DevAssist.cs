using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class DevAssist : EditorWindow
    {
        [MenuItem("DevAssist/Build APK", priority = 0)]
        public static void OpenBuildWindow()
        {
            GetWindow(typeof(BuildEditorWindow)).ShowModalUtility();
        }

        [MenuItem("DevAssist/Open PersistentPath", priority = 1)]
        public static void OpenPersistentPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("DevAssist/Remove Save Data", priority = 2)]
        public static void RemoveSaveData()
        {
            string savePath = $"{Application.persistentDataPath}/Player.json";
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }
    }

    public static class EditorPreset
    {
        public static GUILayoutOption[] SmallLabel = new GUILayoutOption[]
        {
            GUILayout.Width(50)
        };

        public static GUILayoutOption[] MediumLabel = new GUILayoutOption[]
        {
            GUILayout.Width(150)
        };

        public static GUILayoutOption[] LargeLabel = new GUILayoutOption[]
        {
            GUILayout.Width(300)
        };

        public static GUIStyle MediumButton = new GUIStyle
        {
            stretchWidth = true,
            fixedHeight = 50f,
            fontSize = 40,
            alignment = TextAnchor.MiddleCenter,
        };
    }
}