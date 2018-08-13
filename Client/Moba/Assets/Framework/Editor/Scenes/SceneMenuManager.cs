using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/8/13 22:27:14</para>
/// </summary> 
public static class SceneMenuManager
{
    [MenuItem("Assets/Scene/New Scene")]
    static void CreateNewScene()
    {
        EditorInputWindow.show(delegate(string newSceneName)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                
                Debug.LogError("请输入场景名");
                return;
            }
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            string newSceneDir = "Assets/Res/BundleScenes/"+ newSceneName + "/";
            string newScenePath = "Assets/Res/BundleScenes/" + newSceneName + ".unity";
            if (File.Exists(newScenePath))
            {
                EditorUtility.DisplayDialog("", "默认场景\"" + newSceneName + "\"已经存在!", "确定");
            }
            else
            {
                if (!Directory.Exists(newSceneDir))
                    Directory.CreateDirectory(newSceneDir);
                if (!Directory.Exists(newSceneDir + "Materials/"))
                    Directory.CreateDirectory(newSceneDir + "Materials/");
                if (!Directory.Exists(newSceneDir + "Models/"))
                    Directory.CreateDirectory(newSceneDir + "Models/");
                if (!Directory.Exists(newSceneDir + "Textures/"))
                    Directory.CreateDirectory(newSceneDir + "Textures/");
                EditorSceneManager.SaveScene(newScene, newScenePath);
            }
        });
    }
}

