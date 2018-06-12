//
// lua模块代码生成
// Author: zhengnan
// Create: 2018/6/12 14:27:47
// 

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class ModulesGenerater
{
    private const string ViewConfigPath = "Assets/Lua/Config/ViewConfig.lua";

    [MenuItem("Tools/Lua/OpenModuelsWnd")]
    static void OpenModuelsWnd()
    {
        string[] viewConfigLine = FileUtils.GetFileTextLine(ViewConfigPath);

        LuaTable lt = new LuaTable();
        lt.fromTextLine(viewConfigLine);
        Debug.Log(lt.toString());
    }
}

