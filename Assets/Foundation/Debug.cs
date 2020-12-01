using System.Diagnostics;
using UnityEngine;

/// 
/// It overrides UnityEngine.Debug to mute debug messages completely on a platform-specific basis.
/// 
/// Putting this inside of 'Plugins' foloder is ok.
/// 
/// Important:
///     Other preprocessor directives than 'UNITY_EDITOR' does not correctly work.
/// 
/// Note:
///     [Conditional] attribute indicates to compilers that a method call or attribute should be 
///     ignored unless a specified conditional compilation symbol is defined.
/// 
/// See Also: 
///     http://msdn.microsoft.com/en-us/library/system.diagnostics.conditionalattribute.aspx
/// 
/// 2012.11. @kimsama
public static class Log
{
    public static bool IsDebugBuild => UnityEngine.Debug.isDebugBuild;

    #region Dev
    [Conditional("UNITY_EDITOR")]
    public static void Debug(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Debug(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }
    #endregion

    public static void Info(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public static void Info(object message, Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    #region Notice
    public static void Warn(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    public static void Warn(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }
    #endregion

    #region Critical
    public static void Error(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void Error(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    public static void Assert(bool condition, object message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }
    #endregion

    #region Debug.Draw
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }
    #endregion
}