using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRum : MonoBehaviour {

    [MenuItem("Run/Run Game")]
	static void RunGame()
    {
        EditorSceneManager.OpenScene("Assets/Resources/Scenes/Splash.unity", OpenSceneMode.Single);
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
    [MenuItem("Run/Enter Edit UI")]
    static void EnterEditUI()
    {
        EditorSceneManager.OpenScene("Assets/Edit/_Scenes/UIEditor.unity", OpenSceneMode.Single);
        //EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
