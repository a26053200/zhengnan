using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace ExcelExporter
{
    public class ExcelReader
    {
        public delegate void OnRowRead(int rowIndex, List<string> colList);
        private string _path;
        const string Empty = "";
        
        public ExcelReader(string path)
        {
            _path = path;
        }
        public void Read(OnRowRead onRowRead)
        {
            using (FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read))
            {
                //Choose one of either 1 or 2
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
                //...
                //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                while (excelReader.Read())
                {
                    List<string> colList = new List<string>();
                    var colCount = excelReader.FieldCount;
                    for (int i = 0; i < colCount; i++)
                    {
                        if (!excelReader.IsDBNull(i))
                            colList.Add(excelReader[i].ToString());
                        else
                            colList.Add(Empty);
                    }
                    onRowRead(excelReader.Depth, colList);
                }
                
                excelReader.Close();
                excelReader.Dispose();
                stream.Close();
                stream.Dispose();
            }
        }
    }
}