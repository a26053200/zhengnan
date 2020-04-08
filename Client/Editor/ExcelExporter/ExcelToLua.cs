
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < excelFileList.Length; i++)
                    {
                        GenerateLua(excelFileList[i], sb);
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

        private static List<string> headFields;
        //private static List<string> headNames;
        private static List<string> headTypes;
        private const string Type_String = "string";
        private const string Type_Number = "number";
        private static void GenerateLua(string path, StringBuilder sb)
        {
            sb.Clear();
            ExcelReader reader = new ExcelReader(path);
            sb.Append("local Data = {");
            sb.AppendLine();
            reader.Read(delegate(int index, List<string> list)
            {
                ExecuteFile(index, list, sb);
            });
            sb.Append("}");
            sb.AppendLine();
            
            sb.AppendLine(@"
function Data.Get(id)
    if Data[id] == nil then
        logError(string.Format('There is no id = %s data is table <"+ Path.GetFileName(path) + @">', id))
        return nil
    else
        return Data[id]
    end
end

return Data
                ");
            Output(sb, path);
        }
        private static void ExecuteFile(int rowIndex, List<string> list,StringBuilder sb)
        {
            if (rowIndex == 0)
            {
                headFields = list;
            }
            else if (rowIndex == 1)
            {
                headTypes = list;
            }
            else if (rowIndex == 2)
            {
                //headNames = list;
            }
            else
            {
                sb.Append($"    [{list[0]}]");
                sb.Append(" = {");
                for (int i = 0; i < list.Count; i++)
                {
                    if (headTypes[i] == Type_Number)
                    {
                       if (EditUtils.IsNumberic(list[i]))
                            sb.Append($"{headFields[i]} = {list[i]}");
                       else
                           sb.Append($"{headFields[i]} = 0");
                    }
                    else
                        sb.Append($"{headFields[i]} = \"{list[i]}\"");
                    if(i < list.Count - 1)
                        sb.Append(", ");
                }
                sb.Append("},");
                sb.AppendLine();
            }
        }

        private static void ReportError(string msg, string path)
        {
            Debug.LogErrorFormat($"{msg} in file - ", Path.GetFileName(path));
        }
        private static void Output(StringBuilder sb, string path)
        {
           Debug.Log(sb.ToString());
           if (!Directory.Exists(setting.outputPath))
               Directory.CreateDirectory(setting.outputPath);
           EditUtils.SaveUTF8TextFile($"{setting.outputPath}/{Path.GetFileNameWithoutExtension(path)}.lua",sb.ToString());
        }
    }
}