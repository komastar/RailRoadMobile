using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class BuildEditorWindow : EditorWindow
    {
        private int bundleVerCode;
        private string appVersion;

        private void Awake()
        {
            bundleVerCode = PlayerSettings.Android.bundleVersionCode;
            bundleVerCode++;
            appVersion = PlayerSettings.bundleVersion;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                appVersion = EditorGUILayout.TextField(appVersion);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("BundleVersionCode");
                bundleVerCode = EditorGUILayout.IntField(bundleVerCode);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Build"))
                {
                    Build();
                    Close();
                }
                else if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool Build()
        {
            PlayerSettings.keystorePass = "b86tcmDW";
            PlayerSettings.keyaliasPass = "Tile2020!";

            string[] levels = new string[]
            {
                "Assets/Scenes/TitleScene.unity",
                "Assets/Scenes/GameScene.unity"
            };

            var buildResult = BuildPipeline.BuildPlayer(new BuildPlayerOptions()
            {
                scenes = levels,
                target = BuildTarget.Android,
                locationPathName = $"AOS/{PlayerSettings.productName}-{appVersion}.{bundleVerCode}.apk"
            });

            if (0 == buildResult.summary.totalErrors)
            {
                PlayerSettings.bundleVersion = appVersion;
                PlayerSettings.Android.bundleVersionCode = bundleVerCode;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
