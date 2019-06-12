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
        public static string ResFolder = "/Res";
        //public static string ResFolder = "Assets/Res/";
        public static string Suffix = ".bundle";

        public static string File_Meta = ".meta";

        public static string File_Svn = ".svn";

        public static string File_DS_Store = ".DS_Store";

        [MenuItem("Tools/Bundle Build")]
        static void StartBuild()
        {
            FetchAllResources();
        }

        static void FetchAllResources()
        {
            Debug.ClearDeveloperConsole();
            string resDir = Application.dataPath + ResFolder;
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(resDir, "*.*");
            for (int i = 0; i < resFiles.Length; i++)
            {
                FileInfo fileInfo = resFiles[i];
                string lowerName = fileInfo.Name.ToLower();
                string dirPath = BMEditUtility.Absolute2Relativity(fileInfo.DirectoryName) + "/";

                //过滤文件
                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if ((fileInfo.Directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if (lowerName.EndsWith(File_Meta) || fileInfo.Name.EndsWith(File_DS_Store) || dirPath.IndexOf(File_Svn) != -1)
                    continue;

                string path = dirPath + fileInfo.Name; //相对路径

                if(FileType.IsValidFileType(FileType.Texture, lowerName))
                {

                }
                else if (FileType.IsValidFileType(FileType.Material, lowerName))
                {

                }
                else if(FileType.IsValidFileType(FileType.Scene, lowerName))
                {

                }else if(FileType.IsValidFileType(FileType.Model, lowerName))
                {

                } 
                    Logger.Log("path:{0}", path);
                EditorUtility.DisplayProgressBar("Resource Searching...", path, (float)i + 1.0f / (float)resFiles.Length);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
    