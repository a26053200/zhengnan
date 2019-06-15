using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 22:48:38</para>
    /// </summary> 
    public static class BMEditUtility
    {
        static string PathHead = "assets/res/";

        static string Empty = "";

        public static string Path2Name(string path)
        {
            path = path.ToLower();
            string name = path.Replace("\\", "/");
            name = name.Replace(PathHead, Empty);
            name = name.Replace("/", "_");
            return name;
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
        /// <summary>
        /// 获取该目录下面所有的文件,包含子目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">文件后缀名  例如: "(*.jpg|*.bmp)"</param>
        /// <returns></returns>
        public static FileInfo[] GetAllFiles(string path, string searchPattern)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles(searchPattern, SearchOption.AllDirectories);
            return files;
        }

        /// <summary>
        /// 绝对路径->相对路径
        /// </summary>
        public static string Absolute2Relativity(string path)
        {
            string temp = path.Substring(path.IndexOf("Assets"));
            temp = temp.Replace('\\', '/');
            return temp;
        }

        /// <summary>
        /// 相对路径->绝对路径
        /// </summary>
        public static string Relativity2Absolute(string path)
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
            path = files[0].DirectoryName;
            return path;
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

        public static Dictionary<string, string> GetDictionaryFromFile(string fn)
        {
            byte[] data = GetFileData(fn);
            if (data != null)
            {
                ByteReader br = new ByteReader(data);
                return br.ReadDictionary();
            }
            return null;
        }

        public static void SaveDictionary(string fn, Dictionary<string, string> dic)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string k in dic.Keys)
            {
                string v = dic[k];
                sb.Append(string.Format("{0}={1}\r\n", k, v));
            }
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(sb.ToString());
            SaveFileData(fn, data);

        }

        public static void DelFolder(string path)
        {
            if(Directory.Exists(path))
            {
                FileAttributes atr = File.GetAttributes(path);
                if (atr == FileAttributes.Directory)
                    Directory.Delete(path, true);
                else
                    File.Delete(path);
            }
        }
    }
}
    

