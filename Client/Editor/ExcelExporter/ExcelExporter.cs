using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ExcelExporter
{
    public static class ExcelExporter
    {
        static readonly string Setting_Path = "Assets/Res/ExcelExporterSetting.asset";

        public static ExcelExporterSetting LoadSetting()
        {
            if(!File.Exists(Setting_Path))
                EditUtils.CreateAsset<ExcelExporterSetting>(Setting_Path);
            ExcelExporterSetting setting = AssetDatabase.LoadAssetAtPath<ExcelExporterSetting>(Setting_Path);
            return setting;
        }
        
        public static List<string> GetExcelFileList(ExcelExporterSetting setting)
        {
            List<string> excelFileList = new List<string>();
            foreach(string execlFolder in setting.excelFolders)
            {
                DirectoryInfo dirs = new DirectoryInfo(execlFolder);
                FileInfo[] files = dirs.GetFiles(setting.excelPattern, SearchOption.AllDirectories);
            
                for (int i = 0; i < files.Length; i++)
                    excelFileList.Add(execlFolder + "/" + files[i].Name);
               
            }
            if(excelFileList.Count == 0)
                Debug.LogErrorFormat("There is not any excel file in folder");
            return excelFileList;
        }
        
        public static void DisplayProgress(int progress, int total, string file)
        {
            string title = $"Progress..[{progress}/{total}]";
            EditorUtility.DisplayCancelableProgressBar(title, file, (float) progress / (float) total);
        }
    }
}