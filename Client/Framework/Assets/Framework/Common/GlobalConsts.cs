using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>全局常量</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/12 0:25:09</para>
/// </summary> 
public class GlobalConsts
{
    public static bool EnableLogNetwork = true;

    /// <summary>
    /// 是否开启Res AssetBundle模式
    /// </summary>
    public static bool isResBundleMode = false;

    /// <summary>
    /// 是否开启Res AssetBundle模式
    /// </summary>
    public static bool isLuaBundleMode = false;

    /// <summary>
    /// 资源根目录
    /// </summary>
    public static string ResRootDir = "Assets/Res/";

    /// <summary>
    /// lua脚本根目录
    /// </summary>
    public static string LuaRootDir = "Assets/Lua";

    /// <summary>
    /// Tolua脚本根目录
    /// </summary>
    public static string ToLuaRootDir = "Assets/Framework/ToLua/Lua";
}

