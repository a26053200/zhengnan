using System;
using UnityEngine;

namespace BM
{
    public class BMConfig
    {
        public static readonly string BundleDataFile = "BundleData.txt";

        public static readonly string VersionFile = "Version.txt";

        public static Language Language = Language.zh_CN;

        //只读目录(随包走)
#if UNITY_EDITOR
        public static string ReadonlyDir
        {
            get
            {
    #if UNITY_EDITOR_OSX
                string dir = "StandaloneOSX";
    #elif UNITY_EDITOR_WIN
                string dir = "StandaloneWindows64";
    #endif
                string path =  Application.dataPath;
                path = path.Replace("Assets", "AssetBundle/" + Language + "/" + dir);
                return path;
            }
        }
        
#elif UNITY_IOS
        public static readonly string ReadonlyDir = Application.dataPath +"/Raw";
#elif UNITY_ANDROID
        public static readonly string ReadonlyDir = Application.streamingAssetsPath;
#else
        public static readonly string ReadonlyDir = Application.dataPath + "/StreamingAssets";
#endif

        //可读写目录
        public static readonly string RawDir = Application.persistentDataPath + "/";

        //临时可读写目录
        public static readonly string tempCacheRawDir = Application.temporaryCachePath + "/";
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

    public enum BuildType
    {
        Single, //单包
        Scene,  //场景
        Pack,   //整包
        Shader, //Shader
        Lua,    //Lua
    }

    public enum BundleLoadState
    {
        None,
        Loading,
        LoadComplete,
    }

    /// <summary>
    /// 压缩格式
    /// </summary>
    public enum CompressType
    {
        None,   //不做压缩,缺点是打包后体积非常大,不过输入和加载会很快
        LZMA,   //优点是打包后体积小，缺点是解包时间长导致加载时间长
        LZ4,    //优点解压快,解压需要内存小，缺点是打包后体积大
    }
}


