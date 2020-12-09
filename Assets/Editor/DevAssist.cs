using System.Collections;
using System.Collections.Generic;
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