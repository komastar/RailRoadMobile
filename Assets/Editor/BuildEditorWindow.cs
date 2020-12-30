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
        private int prevBundleVerCode;
        private int bundleVerCode;
        private string appVersion;

        private void Awake()
        {
            appVersion = PlayerSettings.bundleVersion;
            prevBundleVerCode = PlayerSettings.Android.bundleVersionCode;
            bundleVerCode = prevBundleVerCode;
            bundleVerCode++;
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

            if (GUILayout.Button("Build"))
            {
                Close();
                Build();
            }
            else if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }

        private bool Build()
        {
            PlayerSettings.Android.bundleVersionCode = bundleVerCode;
            PlayerSettings.bundleVersion = appVersion;

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

            string path = $"AOS/{PlayerSettings.productName}-{appVersion}.{bundleVerCode}.apk";
            var buildResult = BuildPipeline.BuildPlayer(new BuildPlayerOptions()
            {
                scenes = levels,
                target = BuildTarget.Android,
                locationPathName = path
            });

            if (0 == buildResult.summary.totalErrors)
            {
                EditorUtility.RevealInFinder(path);

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
