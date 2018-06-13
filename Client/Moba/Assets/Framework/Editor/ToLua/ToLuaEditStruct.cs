﻿using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/13 22:51:18</para>
/// </summary> 


/// <summary>
///  Lua 模块信息
/// </summary>
public class LuaModuleInfo
{
    public string moduleName;
    public string viewDirPath;
    public List<LuaViewInfo> viewList;

    public LuaModuleInfo(string moduleName)
    {
        viewList = new List<LuaViewInfo>();
        this.moduleName = moduleName;
    }
}
/// <summary>
/// Lua view信息
/// </summary>
public class LuaViewInfo
{
    public string viewName;
    public string viewDirPath;
    public string prefabUrl = "";

    public LuaViewInfo(string viewName)
    {
        this.viewName = viewName;
    }
}