using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace ExcelExporter
{
    public class ExcelReader
    {
        private string _path;
        private readonly string dot = "|";
        private readonly string _NULL = "null";
        public ExcelReader(string path)
        {
            _path = path;
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                //Choose one of either 1 or 2
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
                //...
                //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                StringBuilder sb = new StringBuilder();
                
                while (excelReader.Read())
                {
                    sb.Clear();
                    var colCount = excelReader.FieldCount;
                    sb.Append(excelReader.Depth);
                    for (int i = 0; i < colCount; i++)
                    {
                        if (!excelReader.IsDBNull(i))
                        {
                            sb.Append(excelReader[i].ToString());
                        }
                        else
                        {
                            sb.Append(_NULL);
                        }
                        sb.Append(dot);
                    }
                    Debug.Log(sb.ToString());
                }
            }
        }
    }
}