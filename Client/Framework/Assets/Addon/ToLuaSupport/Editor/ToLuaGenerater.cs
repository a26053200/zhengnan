using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Framework;

namespace ToLuaSupport
{
    /// <summary>
    /// <para>Lua 代码文件生成器</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2018/6/14 1:22:28</para>
    /// </summary> 
    public class ToLuaGenerater
    {
        public static ToLuaSetting _setting;
        
        const string TODO = "--TODO";

        /// <summary>
        /// 生成 Lua 文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="folder">目录</param>
        public static void GeneratedLuaFile(string path, string moduleName, string className, LuaFolder folder)
        {
            string luaFileText = null;
            switch (folder)
            {
                case LuaFolder.Mdr:
                    luaFileText = _setting.LuaMdrClassFile;
                    break;
                case LuaFolder.Model:
                    luaFileText = _setting.LuaModelClassFile;
                    break;
                case LuaFolder.Service:
                    luaFileText = _setting.LuaServicelClassFile;
                    break;
                case LuaFolder.Vo:
                    luaFileText = _setting.LuaVoClassFile;
                    break;
            }

            string instanceName = StringUtils.FirstToLower(className);
            luaFileText = string.Format(luaFileText, SystemUtils.GetSystemUserName(), TimeUtils.NowString());
            LuaFolder moduleFolder = folder == LuaFolder.Mdr ? LuaFolder.View : folder;
            string modulePakerName = GetLuaPackerNameByPath(path + "/"); //模块包名
            string packerName = GetLuaPackerNameByPath(path + Folder2Directory(moduleFolder)); //包名
            luaFileText = StringUtils.ReplaceAll(luaFileText, _setting.MODULE_NAME, moduleName);
            luaFileText = StringUtils.ReplaceAll(luaFileText, _setting.Module_name, StringUtils.FirstToLower(moduleName));
            luaFileText = StringUtils.ReplaceAll(luaFileText, _setting.MODULE_PACKER, modulePakerName);
            luaFileText = StringUtils.ReplaceAll(luaFileText, _setting.CLASS_PACKER, packerName);
            luaFileText = StringUtils.ReplaceAll(luaFileText, _setting.CLASS_NAME, className);
            luaFileText = StringUtils.ReplaceAll(luaFileText, _setting.INSTANCE_NAME, instanceName);
            luaFileText = StringUtils.ReplaceAll(luaFileText, "'", "\"");
            //Debug.Log(mdrFileText);
            string mdrFilePath = path + Folder2Directory(moduleFolder) + className + folder + ".lua";
            if (!File.Exists(mdrFilePath))
            {
                FileUtils.SaveTextFile(mdrFilePath, luaFileText);
                EditorUtility.DisplayDialog("提示", "生成 " + folder + " 文件成功", "确定");
            }
            else
            {
                bool replace = EditorUtility.DisplayDialog("提示", "文件以及存在,是否替换?", "替换", "取消");
                if (replace)
                {
                    FileUtils.SaveTextFile(mdrFilePath, luaFileText);
                    EditorUtility.DisplayDialog("提示", "生成 " + folder + " 文件成功", "确定");
                }
            }
        }

        public static void GeneratedTODOLua(string orgPath, StringBuilder sb)
        {
            string[] textLine = FileUtils.GetFileTextLine(orgPath);
            StringBuilder orgSb = new StringBuilder();
            int state = 0;
            for (int i = 0; i < textLine.Length; i++)
            {
                string line = textLine[i];
                if (state == 0)
                {
                    orgSb.AppendLine(textLine[i]);
                    if (line.IndexOf(TODO) != -1)
                    {
                        state = 1;
                        orgSb.Append(sb.ToString()); //插入
                    }
                }
                else if (state == 1)
                {
                    if (line.IndexOf(TODO) != -1)
                    {
                        orgSb.AppendLine(textLine[i]);
                        state = 2;
                    }
                }
                else if (state == 2)
                {
                    orgSb.AppendLine(textLine[i]);
                }
            }

            Debug.Log(orgSb.ToString());
            FileUtils.SaveTextFile(orgPath, orgSb.ToString());
        }

        const string Format_Mdr_Line = "\tself.binder:Bind(require(\"{0}\")):To(ViewConfig.{1}.name)";
        const string Format_Shingleton_Line = "\tself.binder:Bind(require(\"{0}\")):ToSingleton()";

        //生成lua mvc line
        public static string GetMdrLuaLine(string filePath, string moduleName, LuaFolder folder)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string viewName = fileName.Replace(folder.ToString(), "");
            string packName = string.Format("{0}.{1}.View.{2}",_setting.PackageRoot, moduleName, fileName);
            return string.Format(Format_Mdr_Line, packName, viewName);
        }

        //生成lua mvc line
        public static string GetSingletonLuaLine(string filePath, LuaFolder folder)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string viewName = fileName.Replace(folder.ToString(), "");
            string packName = string.Format("{0}.{1}.{2}.{3}",_setting.PackageRoot, viewName, folder.ToString(), fileName);
            return string.Format(Format_Shingleton_Line, packName);
        }

        //生成文件夹
        public static void GeneratedFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        public static LuaFileStatus GetFileStatus(string moduleDirPath, LuaFolder folder, string fileName = null)
        {
            LuaFolder moduleFolder = folder == LuaFolder.Mdr ? LuaFolder.View : folder;
            moduleDirPath = moduleDirPath + Folder2Directory(moduleFolder);
            if (Directory.Exists(moduleDirPath))
            {
                string[] lusFiles = Directory.GetFiles(moduleDirPath, "*.lua");
                if (lusFiles.Length == 0)
                    return LuaFileStatus.Folder_Only;
                else
                {
                    bool hasFolderFile = false;
                    for (int i = 0; i < lusFiles.Length; i++)
                        if (lusFiles[i].LastIndexOf(folder.ToString()) != -1)
                        {
                            hasFolderFile = true;
                            break;
                        }

                    if (fileName != null)
                    {
                        for (int i = 0; i < lusFiles.Length; i++)
                            if (lusFiles[i].LastIndexOf(fileName) != -1)
                                return LuaFileStatus.Folder_And_TagLuaFile;
                    }

                    if (hasFolderFile)
                        return LuaFileStatus.Folder_And_LuaFile;
                    else
                        return LuaFileStatus.Nothing;
                }
            }

            return LuaFileStatus.Nothing;
        }

        //根据路径获得包名
        static string GetLuaPackerNameByPath(string path)
        {
            path = StringUtils.ReplaceAll(path, "\\", "/");
            path = path.Replace(Application.dataPath + "/Lua/", "");
            string packer = StringUtils.ReplaceAll(path, "\\", ".");
            packer = StringUtils.ReplaceAll(path, "/", ".");
            if (packer.EndsWith("."))
                packer = packer.Substring(0, packer.Length - 1);
            return packer;
        }

        public static string Folder2Directory(LuaFolder folder)
        {
            return "/" + folder.ToString() + "/";
        }

        public static bool FileNameValid(string fileName, EditorWindow wnd = null)
        {
            Regex regex = new Regex("^[0-9]");
            if (string.IsNullOrEmpty(fileName))
            {
                if (wnd)
                    wnd.ShowNotification(new GUIContent("名称不能为空"));
                return false;
            }
            else if (regex.IsMatch(fileName))
            {
                if (wnd)
                    wnd.ShowNotification(new GUIContent("名称不能以数字开头"));
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public enum LuaFolder
    {
        View,
        Mdr,
        Model,
        Service,
        Vo,
    }

    public enum LuaFileStatus
    {
        Nothing, //什么都没有
        Folder_Only, //只有目录
        Folder_And_LuaFile, //目录和目标文件都存在
        Folder_And_TagLuaFile, //目录和目标文件都存在
    }
}