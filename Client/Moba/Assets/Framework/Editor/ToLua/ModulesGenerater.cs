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
using System.Text;

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
    List<LuaModuleInfo> moduleInfoList;
    void Init()
    {
        luaTable = GetLuaTable(ViewConfigPath);
    }
    private void OnGUI()
    {
        if (luaTable != null)
        {
            if(moduleInfoList == null)
            {
                if (GUILayout.Button("Load All Modules"))
                    LoadAllModules();
            }
            else
            {
                EditModulelist();
                if (GUILayout.Button("重新生成 ViewConfig.lua 文件"))
                {
                    string fileText = luaTable.toString();
                    FileUtils.SaveTextFile(ViewConfigPath, fileText);
                    Debug.Log("保存成功 \n" + fileText);
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        moduleName = EditorGUILayout.TextField("模块名称:", moduleName);
        if (!string.IsNullOrEmpty(moduleName) && GUILayout.Button("生成新模块的相关文件夹,并刷新模块列表"))
        {
            if(!luaTable.HasTable(moduleName))
            {
                Dictionary<string, object>  table = luaTable.SetTable(moduleName);
                table.Add("name", moduleName);
                GeneratedModuleFolders(moduleName);
                
                LoadAllModules();
            }
            else
                ShowNotification(new GUIContent("该模块已经存在"));
        }
    }


    //加载所有模块
    public void LoadAllModules()
    {
        string modulesRootDir = Path.Combine(Application.dataPath, LuaModulesDir);
        string[] moduleDirs = Directory.GetDirectories(modulesRootDir);
        if (moduleInfoList == null)
            moduleInfoList = new List<LuaModuleInfo>();
        for (int i = 0; i < moduleDirs.Length; i++)
        {
            string dirPath = moduleDirs[i];
            LuaModuleInfo moduleInfo = new LuaModuleInfo(Path.GetFileName(dirPath));
            moduleInfo.viewDirPath = dirPath + "/View/";
            string[] mdrFiles = Directory.GetFiles(moduleInfo.viewDirPath, "*.lua");
            for (int j = 0; j < mdrFiles.Length; j++)
            {
                string mdrFilePath = mdrFiles[j];
                string mdrName = Path.GetFileNameWithoutExtension(mdrFilePath);
                mdrName = mdrName.Replace("Mdr", "");
                LuaViewInfo viewInfo = new LuaViewInfo(mdrName);
                viewInfo.viewDirPath = moduleInfo.viewDirPath;
                if (luaTable.HasTable(mdrName))
                {//已经存在配置
                    Dictionary<string, object> table = luaTable.SetTable(mdrName);
                    viewInfo.prefabUrl = luaTable.GetString(mdrName, "prefab");
                }
                moduleInfo.viewList.Add(viewInfo);
            }
            moduleInfoList.Add(moduleInfo);
        }
    }
    public void EditModulelist()
    {
        for (int i = 0; i < moduleInfoList.Count; i++)
        {
            LuaModuleInfo moduleInfo = moduleInfoList[i];
            EditorGUILayout.LabelField("Module name:", moduleInfo.moduleName);
            for (int j = 0; j < moduleInfo.viewList.Count; j++)
            {
                EditorGUI.indentLevel++;
                LuaViewInfo viewInfo = moduleInfo.viewList[j];
                if (luaTable.HasTable(viewInfo.viewName))
                {//已经存在配置
                    Dictionary<string, object> table = luaTable.GetTable(viewInfo.viewName);
                    EditorGUILayout.LabelField("View name:", viewInfo.viewName);
                    if(string.IsNullOrEmpty(viewInfo.prefabUrl))
                        viewInfo.prefabUrl = luaTable.GetString(viewInfo.viewName, "prefab");
                    FetchPrefabUrl(viewInfo);
                }
                else
                {
                    Dictionary<string, object> table = luaTable.SetTable(viewInfo.viewName);
                    oldString = viewInfo.viewName;
                    viewInfo.viewName = EditorGUILayout.TextField("View name:", viewInfo.viewName);
                    if(oldString != viewInfo.viewName)
                        luaTable.SetHashTable(viewInfo.viewName, "name", viewInfo.viewName);

                    oldString = viewInfo.prefabUrl;
                    FetchPrefabUrl(viewInfo);
                    if (oldString != viewInfo.prefabUrl)
                        luaTable.SetHashTable(viewInfo.viewName, "prefab", viewInfo.prefabUrl);
                }
                if (GUILayout.Button("重新生成 Mediator 文件"))
                {
                    GeneratedMediatorFile(viewInfo);
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    private void FetchPrefabUrl(LuaViewInfo viewInfo)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefab url:", viewInfo.prefabUrl);
        if (GUILayout.Button("..."))
        {
            string prefabPath = OpenFileDialoger.OpenFile("选择UI预制件", Path.Combine(Application.dataPath, PrefabsRootDir), "prefab");
            if (!string.IsNullOrEmpty(prefabPath))
            {
                string fileName = Path.GetFileNameWithoutExtension(prefabPath);
                fileName = fileName.Replace("Wnd", "");
                fileName = fileName.Replace("Panel", "");
                fileName = fileName.Replace("View", "");
                moduleName = fileName;
                prefabPath = StringUtils.ReplaceAll(prefabPath, "\\", "/");
                prefabPath = prefabPath.Replace(Application.dataPath, "");
                viewInfo.prefabUrl = prefabPath.Substring(1);
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    //生成模块相关文件夹
    public void GeneratedModuleFolders(string moduleName)
    {
        string moduleDir = Path.Combine(Application.dataPath, LuaModulesDir + moduleName + "/");
        if (!Directory.Exists(moduleDir))
            Directory.CreateDirectory(moduleDir);
        if (!Directory.Exists(moduleDir + "View/"))
            Directory.CreateDirectory(moduleDir + "View/");
    }


    //生成模块相关文件夹
    public void GeneratedMediatorFile(LuaViewInfo viewInfo)
    {
        ToLuaGenerater.GeneratedMediatorFile(viewInfo.viewDirPath, viewInfo.viewName);
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

