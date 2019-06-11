using System;
using UnityEngine;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>打包平台</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 0:24:39</para>
    /// </summary> 
    public class Platform
    {
        public static string GetPlatformName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "IOS";
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 语言和地域
    /// </summary>
    public enum Language
    {
        zh_CN,//华 -大陆
        zh_HK,//华 -香港
        zh_TW,//华 -台湾
        zh_SG,//华 -新加坡
        ja_JP,//日本 -日本
        en_US,//英语 - 美国
        en_GB,//英语 - 英国
    }
}


