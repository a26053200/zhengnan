using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameRum
{

    [MenuItem("Tools/Clear Progress Bar",false,1)]
    public static void ClearBuild()
    {
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("Run/Run Game")]
	static void RunGame()
    {
        EditorSceneManager.OpenScene("Assets/Resources/Scenes/Splash.unity", OpenSceneMode.Single);
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    [MenuItem("Run/Switch To Editor Mode")]
    static void SetMode0()
    {
        SetRunMode(1, false);
        SetRunMode(2, false);
    }

    [MenuItem("Run/Switch To Editor Mode", true)]
    static bool RunMode0Enable()
    {
        return GetRunMode(1) == 1 | GetRunMode(2) == 1;
    }

    [MenuItem("Run/Switch To Lua Bundle Mode")]
    static void SetMode1()
    {
        SetRunMode(1, true);
    }

    [MenuItem("Run/Switch To Lua Bundle Mode", true)]
    static bool RunMode1Enable()
    {
        return GetRunMode(1) == 0;
    }

    [MenuItem("Run/Switch To Res Bundle Mode")]
    static void SetMode2()
    {
        SetRunMode(2, true);
    }

    [MenuItem("Run/Switch To Res Bundle Mode", true)]
    static bool RunMode2Enable()
    {
        return GetRunMode(2) == 0;
    }


    static int GetRunMode(int mode)
    {
        return PlayerPrefs.GetInt(StringUtils.EncryptWithMD5(Application.dataPath) + "_Mode" + mode, 0);
    }

    static void SetRunMode(int mode, bool value)
    {
        PlayerPrefs.SetInt(StringUtils.EncryptWithMD5(Application.dataPath) + "_Mode" + mode, value ? 1 : 0);
    }

    [MenuItem("Run/Enter Edit UI")]
    static void EnterEditUI()
    {
        EditorSceneManager.OpenScene("Assets/Edit/Scenes/UIEditor.unity", OpenSceneMode.Single);
        //EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
