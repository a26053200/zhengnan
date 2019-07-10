using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace BM
{
    /// <summary>
    /// <para>打包平台工具集合</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 0:24:39</para>
    /// </summary>
    public class BMUtility
    {
        static string Empty = "";

        public static string Path2Name(string path, string head = "assets/res/")
        {
            path = path.ToLower();
            string name = path.Replace("\\", "/");
            name = name.Replace(head, Empty);
            name = name.Replace("/", "_");
            return name;
        }

        public static string Name2Path(string name, string head = "assets/res/")
        {
            name = name.ToLower();
            string path = name.Replace("_", "/");
            return path;
        }

        public static bool FileExists(string path)
        {
#if UNITY_ANDROID

#else
            return File.Exists(path);
#endif
        }

        public static string LoadText(string path)
        {
#if UNITY_ANDROID
            //java实现的加载
#else
            return File.ReadAllText(path);
#endif
        }

        public static List<string> JsonToArray(JsonData json, string fieldName)
        {
            List<string> list = new List<string>();
            if(json.Keys.Contains(fieldName))
            {
                for (int i = 0; i < json[fieldName].Count; i++)
                {
                    list.Add(json[fieldName][i].ToString());
                }
            }
            return list;
        }

        #region 获取MD5值

        public static string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }

        
        
        #endregion
    }

}
