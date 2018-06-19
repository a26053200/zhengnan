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
    GUILayoutOption endButtonWidth = null;
    string moduleName = "";
    string prefabUrl = "";
    string oldString = "";
    string newViewMdrName = "";
    string newVoName = "";
    int oldInt;
    bool oldBool;
    LuaTable luaTable = null;
    List<LuaModuleInfo> moduleInfoList;
    Vector2 scrollPos = Vector2.zero;
    void Init()
    {
        endButtonWidth = GUILayout.Width(position.width * 0.2f);
        luaTable = GetLuaTable(ViewConfigPath);
        minSize = new Vector2(500,500);
        LoadAllModules();
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
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height * 0.8f));
                EditModulelist();
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("重新生成 ViewConfig.lua 文件"))
                {
                    string fileText = luaTable.ToString();
                    FileUtils.SaveTextFile(ViewConfigPath, fileText);
                    Debug.Log("保存成功 \n" + fileText);
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        moduleName = EditorGUILayout.TextField("新增模块 模块名:", moduleName);
        if (GUILayout.Button("生成新模块", endButtonWidth))
        {
            if(string.IsNullOrEmpty(moduleName))
            {
                ShowNotification(new GUIContent("模块名不能为空"));
                return;
            }
            if (luaTable.HasTable(moduleName))
            {
                ShowNotification(new GUIContent("该模块已经存在"));
                return;
            }

            Dictionary<string, object> table = luaTable.SetTable(moduleName);
            table.Add("name", moduleName);
            GeneratedModuleFolders(moduleName);

            LoadAllModules();
        }
        EditorGUILayout.EndHorizontal();
    }


    //加载所有模块
    void LoadAllModules()
    {
        string modulesRootDir = Path.Combine(Application.dataPath, LuaModulesDir);
        string[] moduleDirs = Directory.GetDirectories(modulesRootDir);
        moduleInfoList = new List<LuaModuleInfo>();
        for (int i = 0; i < moduleDirs.Length; i++)
        {
            string moduleDirPath = moduleDirs[i];//模块目录路径
            LuaModuleInfo moduleInfo = new LuaModuleInfo(Path.GetFileName(moduleDirs[i]));
            moduleInfo.moduleDirPath = moduleDirPath;//模块目录路径
            moduleInfo.viewDirPath = moduleDirPath + ToLuaGenerater.Folder2Directory(LuaFolder.View);
            string[] mdrFiles = Directory.GetFiles(moduleInfo.viewDirPath, "*.lua");
            for (int j = 0; j < mdrFiles.Length; j++)
            {
                string mdrFilePath = mdrFiles[j];
                string mdrName = Path.GetFileNameWithoutExtension(mdrFilePath);
                mdrName = mdrName.Replace("Mdr", "");
                LuaViewInfo viewInfo = new LuaViewInfo(mdrName);
                viewInfo.viewName = mdrName;
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

    void EditModulelist()
    {
        for (int i = 0; i < moduleInfoList.Count; i++)
        {
            LuaModuleInfo moduleInfo = moduleInfoList[i];
            EditorGUILayout.LabelField("模块:", moduleInfo.moduleName);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Views:");
            EditorGUI.indentLevel++;
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
                    oldString = viewInfo.prefabUrl;
                    FetchPrefabUrl(viewInfo);
                    if (oldString != viewInfo.prefabUrl)
                        luaTable.SetHashTable(viewInfo.viewName, "prefab", viewInfo.prefabUrl);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("搜索到新的View:" + viewInfo.viewName + "是否新建?");
                    if (GUILayout.Button("新建", endButtonWidth))
                    {
                        luaTable.SetTable(viewInfo.viewName);
                        luaTable.SetHashTable(viewInfo.viewName, "name", viewInfo.viewName);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    LuaFileStatus status = ToLuaGenerater.GetFileStatus(moduleInfo.moduleDirPath, LuaFolder.Mdr);
                    if (status == LuaFileStatus.Folder_Only)
                    {
                        EditorGUILayout.LabelField("Mdr文件:");
                        if (GUILayout.Button("生成 " + viewInfo.viewName + " 的 Mediator 文件"))
                            ToLuaGenerater.GeneratedLuaFile(moduleInfo.moduleDirPath, moduleInfo.moduleName, viewInfo.viewName, LuaFolder.Mdr);
                    }
                    else if (status == LuaFileStatus.Folder_And_LuaFile)
                        EditorGUILayout.LabelField("Mdr文件:", viewInfo.viewName + LuaFolder.Mdr + ".lua 文件已经生成");
                    else
                        EditorGUILayout.LabelField("Mdr文件:", status.ToString());
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditFolder(moduleInfo, LuaFolder.Model);
            EditFolder(moduleInfo, LuaFolder.Service);
            EditFolder(moduleInfo, LuaFolder.Vo);

            EditorGUILayout.BeginHorizontal();
            newViewMdrName = EditorGUILayout.TextField("新View 名称:", newViewMdrName);
            if (GUILayout.Button("新增", endButtonWidth))
            {
                if (string.IsNullOrEmpty(newViewMdrName))
                {
                    ShowNotification(new GUIContent("View 名称不能为空"));
                    return;
                }
                LuaViewInfo viewInfo = new LuaViewInfo(newViewMdrName);
                viewInfo.viewName = newViewMdrName;
                viewInfo.viewDirPath = moduleInfo.viewDirPath;
                luaTable.SetTable(viewInfo.viewName);
                luaTable.SetHashTable(viewInfo.viewName, "name", viewInfo.viewName);
                moduleInfo.viewList.Add(viewInfo);
                newViewMdrName = "";
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }
    }

    void EditFolder(LuaModuleInfo moduleInfo, LuaFolder folder)
    {
        string folderPath = moduleInfo.moduleDirPath + ToLuaGenerater.Folder2Directory(folder);
        LuaFileStatus status = ToLuaGenerater.GetFileStatus(moduleInfo.moduleDirPath, folder);
        switch (status)
        {
            case LuaFileStatus.Nothing:
                if (GUILayout.Button("生成 "+ folder + " 文件夹"))
                    ToLuaGenerater.GeneratedFolder(folderPath);
                break;
            case LuaFileStatus.Folder_Only:
                if (folder == LuaFolder.Vo)
                    AddVoFile(moduleInfo);
                else if (GUILayout.Button("生成 " + folder + ".lua 文件"))
                    ToLuaGenerater.GeneratedLuaFile(moduleInfo.moduleDirPath, moduleInfo.moduleName, moduleInfo.moduleName, folder);
                break;
            case LuaFileStatus.Folder_And_LuaFile:
                if (folder == LuaFolder.Vo)
                    AddVoFile(moduleInfo);
                else
                    EditorGUILayout.LabelField(moduleInfo.moduleName + folder + ".lua 文件已经生成");
                break;
        }
    }

    void AddVoFile(LuaModuleInfo moduleInfo)
    {
        EditorGUILayout.BeginHorizontal();
        newVoName = EditorGUILayout.TextField("新Vo 名称:", newVoName);
        if (GUILayout.Button("生成 " + newVoName + "Vo.lua 文件"))
        {
            if (string.IsNullOrEmpty(newVoName))
            {
                ShowNotification(new GUIContent("Vo 名称不能为空"));
                return;
            }
            ToLuaGenerater.GeneratedLuaFile(moduleInfo.moduleDirPath, moduleInfo.moduleName, newVoName, LuaFolder.Vo);
        }
        EditorGUILayout.EndHorizontal();
    }
    void FetchPrefabUrl(LuaViewInfo viewInfo)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefab url:", viewInfo.prefabUrl);
        if (GUILayout.Button("...", endButtonWidth))
        {
            string prefabPath = EditorUtility.OpenFilePanel("选择UI预制件", Path.Combine(Application.dataPath, PrefabsRootDir), "prefab");
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
    void GeneratedModuleFolders(string moduleName)
    {
        string moduleDir = Path.Combine(Application.dataPath, LuaModulesDir + moduleName + "/");
        if (!Directory.Exists(moduleDir))
            Directory.CreateDirectory(moduleDir);
        if (!Directory.Exists(moduleDir + "View/"))
            Directory.CreateDirectory(moduleDir + "View/");
    }

    LuaTable GetLuaTable(string path)
    {
        string[] textLine = FileUtils.GetFileTextLine(path);
        if(textLine.Length > 0)
        {
            LuaTable lt = new LuaTable();
            lt.fromTextLine(textLine);
            Debug.Log("获取LuaTable -- " + lt.ToString());
            return lt;
        }
        else
        {
            return null;
        }
    }
}

