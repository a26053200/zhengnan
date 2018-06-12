//
// lua模块代码生成
// Author: zhengnan
// Create: 2018/6/12 14:27:47
// 

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ModulesGenerater : EditorWindow
{
    private const string ViewConfigPath = "Assets/Lua/Config/ViewConfig.lua";
    private const string LuaModulesDir = "Lua/Modules/";
    private const string PrefabsDir = "Assets/Res/Prefabs/UI/";
    private const string PrefabsRootDir = "Res/Prefabs/UI/";
    [MenuItem("Tools/Lua/Generated Ioc MVC files")]
    static void Generated()
    {
        
    }

    [MenuItem("Tools/Lua/OpenModuelsWnd")]
    static void OpenModuelsWnd()
    {
        ModulesGenerater wnd = EditorWindow.GetWindow<ModulesGenerater>("New Module");
        wnd.Show();
        wnd.Init();
    }

    string moduleName = "";
    string prefabUrl = "";
    string oldString;
    int oldInt;
    bool oldBool;
    LuaTable luaTable = null;
    void Init()
    {
        luaTable = GetLuaTable(ViewConfigPath);
    }
    private void OnGUI()
    {
        if (luaTable != null)
        {
         
        }
        oldString = moduleName;
        moduleName = EditorGUILayout.TextField("模块名称:", moduleName);
        if (moduleName != oldString)
        {

        }
        EditorGUILayout.BeginHorizontal();
        prefabUrl = EditorGUILayout.TextField("模块预制件路径:", prefabUrl);
        if (prefabUrl != oldString)
        {

        }
        if(GUILayout.Button("..."))
        {
            string prefabPath = OpenFileDialoger.OpenFile("选择UI预制件", Path.Combine(Application.dataPath , PrefabsRootDir), "prefab");
            if(!string.IsNullOrEmpty(prefabPath))
            {
                string fileName = Path.GetFileNameWithoutExtension(prefabPath);
                fileName = fileName.Replace("Wnd","");
                fileName = fileName.Replace("Panel", "");
                fileName = fileName.Replace("View", "");
                moduleName = fileName;
                prefabPath = StringUtils.ReplaceAll(prefabPath, "\\", "/");
                prefabPath = prefabPath.Replace(Application.dataPath, "");
                prefabUrl = prefabPath.Substring(1);
            }
        }
        EditorGUILayout.EndHorizontal();
        if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(prefabUrl) && GUILayout.Button("生成模块相关文件夹及文件,并保存到ViewConfig"))
        {
            if(!luaTable.HasTable(moduleName))
            {
                Dictionary<string, object>  table = luaTable.SetTable(moduleName);
                table.Add("name", moduleName);
                table.Add("prefab", prefabUrl);
                Debug.Log("保存成功 -- \n" + luaTable.toString());
                GeneratedModuleFiles(moduleName);
            }
        }
    }

    public void GeneratedModuleFiles(string moduleName)
    {
        string moduleDir = Path.Combine(Application.dataPath, LuaModulesDir + moduleName + "/");
        if (!Directory.Exists(moduleDir))
            Directory.CreateDirectory(moduleDir);
        if (!Directory.Exists(moduleDir + "View/"))
            Directory.CreateDirectory(moduleDir + "View/");
    }

    public void RefreshModules()
    {
        string modulesRootDir = Path.Combine(Application.dataPath, LuaModulesDir);
        string[] moduleDirs = Directory.GetDirectories(modulesRootDir);
        for (int i = 1; i < moduleDirs.Length; i++)
        {
            string dir = moduleDirs[i];
        }
    }

    public LuaTable GetLuaTable(string path)
    {
        string[] textLine = FileUtils.GetFileTextLine(path);
        if(textLine.Length > 0)
        {
            LuaTable lt = new LuaTable();
            lt.fromTextLine(textLine);
            Debug.Log("获取LuaTable -- " + lt.toString());
            return lt;
        }
        else
        {
            return null;
        }
    }
}

