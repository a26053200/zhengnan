using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/13 22:51:18</para>
/// </summary> 


namespace ToLuaSupport
{
    /// <summary>
    ///  Lua 模块信息
    /// </summary>
    public class LuaModuleInfo
    {
        public string moduleName;
        public string moduleDirPath; //Module目录
        public string viewDirPath; //View目录
        public string modelDirPath; //Model目录
        public string serviceDirPath; //Service目录
        public string voDirPath; //Vo目录
        public string commandDirPath; //Command目录
        public List<LuaViewInfo> viewList;
        public List<string> voList;
        public List<string> cmdList;
        
        public bool isOpen;
        public string newViewMdrName { get; set; }

        public LuaModuleInfo(string moduleName)
        {
            viewList = new List<LuaViewInfo>();
            voList = new List<string>();
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
        public string moduleName;

        public Object prefab;
        public LuaViewInfo(string viewName)
        {
            this.viewName = viewName;
        }
    }
}