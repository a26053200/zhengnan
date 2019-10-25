using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.Events;

namespace ToLuaSupport
{
    //
    // Class Introduce
    // Author: zhengnan
    // Create: 2018/5/27 1:55:31
    // 
    public class EditorUtils
    {
        public static void DisplayProgressBar<T>(string title, T[] dataList, UnityAction<T> progressFun)
        {
            for (int i = 0; i < dataList.Length; i++)
            {
                progressFun(dataList[i]);
                EditorUtility.DisplayProgressBar(title, string.Format("{0}/{1}", i + 1, dataList.Length),
                    (float) (i + 1) / (float) dataList.Length);
            }

            EditorUtility.ClearProgressBar();
        }

        public static void DisplayProgressBar<T>(string title, List<T> dataList, UnityAction<T> progressFun)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                progressFun(dataList[i]);
                EditorUtility.DisplayProgressBar(title, string.Format("{0}/{1}", i + 1, dataList.Count),
                    (float) (i + 1) / (float) dataList.Count);
            }

            EditorUtility.ClearProgressBar();
        }
        
        /// <summary>
        /// 创建asset配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public static void CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Not select files, select files first! ");
                return;
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        //横向分割线
        public static void DrawHorizontalSplitter(float height = 5)
        {
            GUILayout.Box("",
                GUILayout.Height(height),
                GUILayout.MaxHeight(height),
                GUILayout.MinHeight(height),
                GUILayout.ExpandWidth(true));
        }

        //纵向向分割线
        public static void DrawVerticalSplitter(float width = 5)
        {
            GUILayout.Box("",
                GUILayout.Width(width),
                GUILayout.MaxWidth(width),
                GUILayout.MinWidth(width),
                GUILayout.ExpandHeight(true));
        }
    }
}
