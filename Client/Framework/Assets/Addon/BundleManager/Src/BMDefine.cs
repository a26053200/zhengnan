using System;
using UnityEngine;

namespace BM
{

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


