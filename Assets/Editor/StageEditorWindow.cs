using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageEditorWindow : EditorWindow
{
    [MenuItem("DevAssist/Open StageEditorWindow")]
    public static void Open()
    {
        var window = GetWindow(typeof(StageEditorWindow));
        window.Show();
    }

#region UNITY_METHODS
    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != "MapScene")
        {
            Close();

            return;
        }

        EditorGUILayout.LabelField("#SCRIPTNAME#");
    }
#endregion
}
