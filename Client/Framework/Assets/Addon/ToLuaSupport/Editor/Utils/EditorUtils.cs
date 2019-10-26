using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
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
        public static string[] GetFileTextLine(string fn)
        {
            if (!File.Exists(fn))
                return null;
            StreamReader sr = File.OpenText(fn);
            List<string> sList = new List<string>();
            try
            {
                while (!sr.EndOfStream)
                    sList.Add(sr.ReadLine());
            }
            finally
            {
                sr.Close();
            }
            return sList.ToArray();
        }


        public static void SaveTextFile(string fn, string txt)
        {
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(txt);
            SaveFileData(fn, data);
        }
        public static void SaveUTF8TextFile(string fn, string txt)
        {
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(txt);
            byte[] bom = new byte[] { 0xef, 0xbb, 0xbf };
            byte[] saveData = new byte[data.Length + bom.Length];
            Array.Copy(bom, 0, saveData, 0, bom.Length);
            Array.Copy(data, 0, saveData, bom.Length, data.Length);
            SaveFileData(fn, saveData);
        }
        public static byte[] GetFileData(string fn)
        {
            if (!File.Exists(fn))
                return null;
            FileStream fs = new FileStream(fn, FileMode.Open);
            try
            {
                if (fs.Length > 0)
                {
                    byte[] data = new byte[(int)fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    return data;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                fs.Close();
            }
        }

        public static void SaveFileData(string fn, byte[] data)
        {
            string dir = Path.GetDirectoryName(fn);
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dir);
            if (!dirinfo.Exists)
                dirinfo.Create();
            FileStream fs = new FileStream(fn, FileMode.Create);
            try
            {
                fs.Write(data, 0, data.Length);
            }
            finally
            {
                fs.Close();
            }
        }
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
