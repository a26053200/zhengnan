using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using System.IO;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/6 0:39:50</para>
    /// </summary> 
    public static class ResUtils
    {
        //Style
        //static GUIStyle labelDefault;
        static GUIStyle labelRed;
        static GUIStyle labelGreen;
        static GUIStyle labelYellow;

        static ResUtils()
        {
            labelRed = new GUIStyle();
            labelRed.normal.textColor = Color.red;

            labelGreen = new GUIStyle();
            labelGreen.normal.textColor = Color.green;

            labelYellow = new GUIStyle();
            labelYellow.normal.textColor = Color.yellow;
        }

        public static void ColorLabelField(string title, string content, int level = 0, int width = 0)
        {
            GUIStyle labelStyle = labelGreen;
            if (level == 1)
                labelStyle = labelYellow;
            else if (level == 2)
                labelStyle = labelRed;
            EditorGUILayout.LabelField(title + ": " + content, labelStyle, width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(width));
        }

        public static void ColorLabelFieldTooltip(string title, string content,string tooltip, int level = 0, int width = 0)
        {
            GUIStyle labelStyle = labelGreen;
            if (level == 1)
                labelStyle = labelYellow;
            else if (level == 2)
                labelStyle = labelRed;
            //EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField(new GUIContent(title), new GUIContent(content, tooltip), labelStyle, width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(width));
            //EditorGUILayout.TextField(new GUIContent(title, tooltip), content, labelStyle, width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(width));
            //EditorGUI.EndDisabledGroup();
        }

        public static void ColorLabelField(string title, string content, bool vaild, int width = 0)
        {
            EditorGUILayout.LabelField(title + ": "+ content, vaild ? labelGreen : labelRed, width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(width));
        }
        public static void ColorLabelFieldTooltip(string title, string content, string tooltip, bool vaild,  int width = 0)
        {
            if (labelRed == null)
            {
                labelRed = new GUIStyle();
                labelRed.normal.textColor = Color.red;

                labelGreen = new GUIStyle();
                labelGreen.normal.textColor = Color.green;
            }
            EditorGUILayout.LabelField(new GUIContent(title + ": " + content, tooltip), vaild ? labelGreen : labelRed, width == 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(width));
        }
        #region 获取相同的文件
        /// <summary>
        /// 获取相同的文件
        /// </summary>
        /// <param name="_PathValue"></param>
        /// <param name="assetsPaths"></param>
        /// <returns></returns>
        public static string[] GetSameFilePaths(string _PathValue, List<string> assetsPaths)
        {
            List<string> samePaths = new List<string>();

            string _AssetMD5 = ResUtils.GetFileMD5(_PathValue);
            //遍历所有Assets
            for (int i = 0; i < assetsPaths.Count; i++)
            {
                if (assetsPaths[i] == _PathValue)
                    continue;
                if (_AssetMD5 == ResUtils.GetFileMD5(assetsPaths[i]))
                    samePaths.Add(assetsPaths[i]);

            }
            return samePaths.ToArray();
        }
        #endregion


        #region 获取其他引用Assets的路径
        public static string[] GetUseAssetPaths(string assetPath, List<string> allAssetsPaths)
        {
            List<string> assetPaths = new List<string>();
            //使用GUID作为判断标准
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            //遍历所有Assets
            for (int i = 0; i < allAssetsPaths.Count; i++)
            {
                if (allAssetsPaths[i] == assetPath)
                    continue;

                string[] _OtherPaths = AssetDatabase.GetDependencies(allAssetsPaths[i]);
                if (_OtherPaths.Length > 1)
                {
                    for (int j = 0; j < _OtherPaths.Length; j++)
                    {
                        string _OtherGUID = AssetDatabase.AssetPathToGUID(_OtherPaths[j]);
                        if (assetGUID == _OtherGUID)
                        {
                            assetPaths.Add(allAssetsPaths[i]);
                        }
                    }
                }
            }
            return assetPaths.ToArray();
        }
        #endregion

        #region 获取文件的MD5值
        /// <summary>
        /// 获取文件的MD5值
        /// </summary>
        /// <param name="_PathValue"></param>
        /// <returns></returns>
        public static string GetFileMD5(string _PathValue)
        {
            //判断是否为本地资源   因为本地文件里有文件名称 但是在资源名称又不能重复  于是需要去掉名称 来检测md5值
            Object _ObejctValue = AssetDatabase.LoadAssetAtPath<Object>(_PathValue);
            bool _isNative = AssetDatabase.IsNativeAsset(_ObejctValue);
            string _FileMD5 = "";
            string _TemPath = Application.dataPath.Replace("Assets", "");

            if (_isNative)
            {
                string _TempFileText = File.ReadAllText(_TemPath + _PathValue).Replace("m_Name: " + _ObejctValue.name, "");

                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                //将字符串转换为字节数组  
                byte[] fromData = System.Text.Encoding.Unicode.GetBytes(_TempFileText);
                //计算字节数组的哈希值  
                byte[] toData = md5.ComputeHash(fromData);
                _FileMD5 = "";
                for (int i = 0; i < toData.Length; i++)
                {
                    _FileMD5 += toData[i].ToString("x2");
                }
            }
            else
            {
                _FileMD5 = "";
                //外部文件的MD5值
                try
                {

                    FileStream fs = new FileStream(_TemPath + _PathValue, FileMode.Open);

                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(fs);
                    fs.Close();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        _FileMD5 += retVal[i].ToString("x2");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex);
                }
                //因为外部文件还存在不同的设置问题，还需要检测一下外部资源的.meta文件
                if (_FileMD5 != "")
                {
                    string _MetaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(_PathValue);
                    string _ObjectGUID = AssetDatabase.AssetPathToGUID(_PathValue);
                    //去掉guid来检测
                    string _TempFileText = File.ReadAllText(_TemPath + _MetaPath).Replace("guid: " + _ObjectGUID, "");

                    System.Security.Cryptography.MD5 _MetaMd5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    //将字符串转换为字节数组  
                    byte[] fromData = System.Text.Encoding.Unicode.GetBytes(_TempFileText);
                    //计算字节数组的哈希值  
                    byte[] toData = _MetaMd5.ComputeHash(fromData);
                    for (int i = 0; i < toData.Length; i++)
                    {
                        _FileMD5 += toData[i].ToString("x2");
                    }
                }
            }
            return _FileMD5;
        }
        #endregion

        #region Hash比较
        public static bool CompareFile(string path1, string path2)
        {
            string _TemPath = Application.dataPath.Replace("Assets", "");
            string p_1 = _TemPath + path1;
            string p_2 = _TemPath + path2;
            //计算第一个文件的哈希值
            var hash = System.Security.Cryptography.HashAlgorithm.Create();
            var stream_1 = new System.IO.FileStream(p_1, System.IO.FileMode.Open);
            byte[] hashByte_1 = hash.ComputeHash(stream_1);
            stream_1.Close();
            //计算第二个文件的哈希值
            var stream_2 = new System.IO.FileStream(p_2, System.IO.FileMode.Open);
            byte[] hashByte_2 = hash.ComputeHash(stream_2);
            stream_2.Close();

            //比较两个哈希值
            if (BitConverter.ToString(hashByte_1) == BitConverter.ToString(hashByte_2))
                return true;
            else
                return false;

        }
        #endregion

        #region IsActs
        public static bool IsActs(string path1, string path2)
        {
            string _TemPath = Application.dataPath.Replace("Assets", "");
            string p_1 = _TemPath + path1;
            string p_2 = _TemPath + path2;
            //计算第一个文件的哈希值
            var hash = System.Security.Cryptography.HashAlgorithm.Create();
            var stream_1 = new System.IO.FileStream(p_1, System.IO.FileMode.Open);
            byte[] hashByte_1 = hash.ComputeHash(stream_1);
            stream_1.Close();
            //计算第二个文件的哈希值
            var stream_2 = new System.IO.FileStream(p_2, System.IO.FileMode.Open);
            byte[] hashByte_2 = hash.ComputeHash(stream_2);
            stream_2.Close();

            //比较两个哈希值
            if (BitConverter.ToString(hashByte_1) == BitConverter.ToString(hashByte_2))
                return true;
            else
                return false;

        }
        #endregion
    }

}

