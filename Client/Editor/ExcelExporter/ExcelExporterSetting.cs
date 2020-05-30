using UnityEditor;
using UnityEngine;

namespace ExcelExporter
{
    public class ExcelExporterSetting : ScriptableObject
    {
        public string[] excelFolders = new string[0];

        public string excelPattern = "*.xlsx";
        
        public string outputPath = "";
        
        public string jsonOutputPath = "";

        public string[] fileList;
    }
}