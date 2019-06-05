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
    }

}

