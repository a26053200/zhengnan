
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ExcelExporter
{
    public static class ExcelToJson 
    {
        static readonly string Setting_Path = "Assets/Res/ExcelExporterSetting.asset";

        private static ExcelExporterSetting setting;
        static void LoadSetting()
        {
            if(!File.Exists(Setting_Path))
                EditUtils.CreateAsset<ExcelExporterSetting>(Setting_Path);
            setting = AssetDatabase.LoadAssetAtPath<ExcelExporterSetting>(Setting_Path);
        }
        //[MenuItem("Tools/ExcelToJson %#j")]
        static void DoExcelToLua()
        {
            setting = ExcelExporter.LoadSetting();
            List<string> excelFileList = ExcelExporter.GetExcelFileList(setting);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < excelFileList.Count; i++)
            {
                GenerateLua(excelFileList[i], sb);
                ExcelExporter.DisplayProgress(i,excelFileList.Count, excelFileList[i]);
            }
            EditorUtility.ClearProgressBar();
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
            sb.Append("{");
            sb.AppendLine();
            reader.Read(delegate(int index, List<string> list)
            {
                ExecuteFile(index, list, sb);
            });
            sb.Append("}");
            sb.AppendLine();
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
                sb.Append($"    \"{list[0]}\"");
                sb.Append(": {");
                for (int i = 0; i < list.Count; i++)
                {
                    if (headTypes[i] == Type_Number)
                    {
                       if (EditUtils.IsNumberic(list[i]))
                            sb.Append($"\"{headFields[i]}\": {list[i]}");
                       else
                           sb.Append($"\"{headFields[i]}\": 0");
                    }
                    else
                        sb.Append($"\"{headFields[i]}\": \"{list[i]}\"");
                    if(i < list.Count - 1)
                        sb.Append(", ");
                }
                sb.Append("},");
                sb.AppendLine();
            }
        }

        private static void Output(StringBuilder sb, string path)
        {
           Debug.Log(sb.ToString());
           if (!Directory.Exists(setting.jsonOutputPath))
               Directory.CreateDirectory(setting.jsonOutputPath);
           EditUtils.SaveUTF8TextFile($"{setting.jsonOutputPath}/{Path.GetFileNameWithoutExtension(path)}.json",sb.ToString());
        }
    }
}