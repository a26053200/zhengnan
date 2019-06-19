using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using LitJson;
using System;
using System.Collections.Generic;

namespace BM
{
    /// <summary>
    /// <para>打包平台工具集合</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 0:24:39</para>
    /// </summary>
    public class BMUtility
    {
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
    }

}
