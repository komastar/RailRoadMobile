using Assets.Foundation.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
            PlayerSettings.bundleVersion = appVersion;
            bundleVerCode = PlayerSettings.Android.bundleVersionCode++;

            var devEnvString = File.ReadAllText($"{Application.dataPath}/.DevEnv/DevEnv.json");
            var devEnv = JObject.Parse(devEnvString).ToObject<DevEnv>();
            PlayerSettings.keystorePass = devEnv.KeyStorePass;
            PlayerSettings.keyaliasPass = devEnv.KeyAliasPass;

            string[] levels = new string[]
            {
                "Assets/Scenes/TitleScene.unity",
                "Assets/Scenes/LobbyScene.unity",
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
                return true;
            }
            else
            {
                PlayerSettings.Android.bundleVersionCode = bundleVerCode;
                return false;
            }
        }
    }
}
