using UnityEngine;


public class MyDebug
{
    public static bool EnableDebug = true;

    public static bool EnableDebugError = true;

    // 摘要: 
    //     Logs message to the Unity Console.
    public static void Log(object message)
    {
        if (EnableDebug)
            Debug.Log(message);
    }
    public static void uLog(object message)
    {
        if (EnableDebug)
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                Debug.Log(string.Format("<color=#00ff00ff>[Lua]</color>{0}", message));
            else
                Debug.Log(string.Format("[Lua]{0}", message));
    }
    public static void uLogWarning(object message)
    {
        if (EnableDebug)
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                Debug.Log(string.Format("<color=#ffff00ff>[Lua]</color>{0}", message));
            else
                Debug.Log(string.Format("[Lua Warning]{0}", message));
    }
    public static void uLogError(object message)
    {
        if (EnableDebug)
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                Debug.Log(string.Format("<color=#ff0000ff>[Lua]</color>{0}", message));
            else
                Debug.Log(string.Format("[Lua Error]{0}", message));
    }
    public static void mLog(object message)
    {
        if (EnableDebug)
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                Debug.Log(string.Format("<color=#98e165ff>[MMO]</color>{0}", message));
            else
                Debug.Log(string.Format("[MMO]{0}", message));
    }
    public static void mLogWarning(object message)
    {
        if (EnableDebug)
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                Debug.Log(string.Format("<color=#ffce45ff>[MMO]</color>{0}", message));
            else
                Debug.Log(string.Format("[MMO Warning]{0}", message));
    }
    public static void mLogError(object message)
    {
        if (EnableDebug)
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                Debug.Log(string.Format("<color=#dd5044ff>[MMO]</color>{0}", message));
            else
                Debug.Log(string.Format("[MMO Error]{0}", message));
    }
    //
    // 摘要: 
    //     Logs message to the Unity Console.
    public static void Log(object message, Object context)
    {
        if (EnableDebug)
            Debug.Log(message, context);
    }
    //
    // 摘要: 
    //     A variant of Debug.Log that logs an error message to the console.
    public static void LogError(object message)
    {
        if (EnableDebug)
            Debug.LogError(message);
    }
    //
    // 摘要: 
    //     A variant of Debug.Log that logs an error message to the console.
    public static void LogError(object message, Object context)
    {
        if (EnableDebugError)
            Debug.LogError(message, context);
    }
    //
    // 摘要: 
    //     A variant of Debug.Log that logs a warning message to the console.
    public static void LogWarning(object message)
    {
        if (EnableDebug)
            Debug.LogWarning(message);
    }
    //
    // 摘要: 
    //     A variant of Debug.Log that logs a warning message to the console.
    public static void LogWarning(object message, Object context)
    {
        if (EnableDebug)
            Debug.LogWarning(message, context);
    }
}

