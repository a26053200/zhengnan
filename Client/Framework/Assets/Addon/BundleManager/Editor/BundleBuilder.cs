using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>Bundle Builder</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 0:24:07</para>
    /// </summary> 
    public static class BundleBuilder
    {
        [MenuItem("Tools/Bundle Build")]
        static void StartBuild()
        {
            FetchAllResources();
        }

        //配置路径
        static string BMSettings_Path = "Assets/Res/BMSettings.txt";

        //配置
        static BMSettings settings;

        public static void Build()
        {
            LoadSetting();
            FetchAllResources();
        }

        static void LoadSetting()
        {
            settings = BMSettings.GetIntance();
            settings.LoadSettings();
        }

        static void FetchAllResources()
        {
            Debug.ClearDeveloperConsole();
            string resDir = BMEditUtility.Relativity2Absolute(settings.ResFolder);
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(resDir, "*.*");
            for (int i = 0; i < resFiles.Length; i++)
            {
                FileInfo fileInfo = resFiles[i];
                string lowerName = fileInfo.Name.ToLower();
                string dirPath = BMEditUtility.Absolute2Relativity(fileInfo.DirectoryName) + "/";

                //过滤一些文件
                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if ((fileInfo.Directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if (IgnoreFile(lowerName, dirPath))
                    continue;

                string path = dirPath + fileInfo.Name; //相对路径

                BundleFileInfo bundleFileInfo = new BundleFileInfo()
                {
                    path = path,
                };
                if (dirPath.IndexOf(settings.Folder_Scenes) != -1)
                {
                    bundleFileInfo.type = BundleType.Scene.ToString();
                }
                else
                {

                }
                Logger.Log("path:{0}", path);
                EditorUtility.DisplayProgressBar("Resource Searching...", path, (float)i + 1.0f / (float)resFiles.Length);
            }
            EditorUtility.ClearProgressBar();
        }

        //=======================
        // 工具函数
        //=======================
        //忽略文件
        static bool IgnoreFile(string lowerName, string dirPath)
        {
            string[] ignoreSuffixs = settings.Ignore_Suffix.Split(',');
            for (int i = 0; i < ignoreSuffixs.Length; i++)
            {
                if (lowerName.EndsWith(ignoreSuffixs[i]))
                    return true;
            }
            string[] ignoreFolders = settings.Ignore_Folder.Split(',');
            for (int i = 0; i < ignoreFolders.Length; i++)
            {
                if (dirPath.IndexOf(ignoreFolders[i]) != -1)
                    return true;
            }
            return false;
        }
    }
}
    