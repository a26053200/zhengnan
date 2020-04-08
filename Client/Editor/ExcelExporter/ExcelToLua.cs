
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ExcelExporter
{
    public static class ExcelToLua
    {
        static readonly string Setting_Path = "Assets/Res/ExcelExporterSetting.asset";

        private static ExcelExporterSetting setting;
        static void LoadSetting()
        {
            if(!File.Exists(Setting_Path))
                EditUtils.CreateAsset<ExcelExporterSetting>(Setting_Path);
            setting = AssetDatabase.LoadAssetAtPath<ExcelExporterSetting>(Setting_Path);
        }
        [MenuItem("Tools/Excel2Lua %#e")]
        static void DoExcelToLua()
        {
            LoadSetting();
            foreach(string execlFolder in setting.excelFolders)
            {
                DirectoryInfo dirs = new DirectoryInfo(execlFolder);
                FileInfo[] files = dirs.GetFiles(setting.excelPattern, SearchOption.AllDirectories);
            
                string[] excelFileList = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    excelFileList[i] = execlFolder + "/" + files[i].Name;
                }
            
                if(excelFileList.Length == 0)
                    Debug.LogErrorFormat("There is not any excel file in folder:{0}", execlFolder);
                else
                {
                    for (int i = 0; i < excelFileList.Length; i++)
                    {
                        ExecuteFile(excelFileList[i]);
                        DisplayProgress(i,excelFileList.Length,execlFolder, excelFileList[i]);
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            
        }

        static void DisplayProgress(int progress, int total, string execlFolder, string file)
        {
            string path = file.Replace(execlFolder, "");
            string title = $"Progress..[{progress}/{total}]";
            EditorUtility.DisplayCancelableProgressBar(title, path, (float) progress / (float) total);
        }
        private static void ExecuteFile(string path)
        {
            ExcelReader reader = new ExcelReader(path);
        }
    }
}