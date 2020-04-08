using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExcelExporter
{
    public class EditUtils
    {
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
    }
}