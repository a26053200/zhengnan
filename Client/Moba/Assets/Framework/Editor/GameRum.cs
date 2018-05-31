using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRum : MonoBehaviour {

    [MenuItem("Run/Run Game")]
	static void RunGame()
    {
        SceneManager.LoadScene("Splash");
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
