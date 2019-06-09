using System.Collections.Generic;
using System.IO;
using System;

namespace Framework
{

    public class FileUtils
    {
        public static string GetTextFile(string fn)
        {
            byte[] data = GetFileData(fn);
            if (data != null)
            {
                if (data.Length > 0)
                {
                    return System.Text.UTF8Encoding.UTF8.GetString(data);
                }
            }
            return null;
        }

        public static string[] GetFileTextLine(string fn)
        {
            if (!File.Exists(fn))
                return null;
            StreamReader sr = File.OpenText(fn);
            List<string> sList = new List<string>();
            try
            {
                while (!sr.EndOfStream)
                    sList.Add(sr.ReadLine());
            }
            finally
            {
                sr.Close();
            }
            return sList.ToArray();
        }


        public static void SaveTextFile(string fn, string txt)
        {
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(txt);
            SaveFileData(fn, data);
        }
        public static void SaveUTF8TextFile(string fn, string txt)
        {
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(txt);
            byte[] bom = new byte[] { 0xef, 0xbb, 0xbf };
            byte[] saveData = new byte[data.Length + bom.Length];
            Array.Copy(bom, 0, saveData, 0, bom.Length);
            Array.Copy(data, 0, saveData, bom.Length, data.Length);
            SaveFileData(fn, saveData);
        }
        public static byte[] GetFileData(string fn)
        {
            if (!File.Exists(fn))
                return null;
            FileStream fs = new FileStream(fn, FileMode.Open);
            try
            {
                if (fs.Length > 0)
                {
                    byte[] data = new byte[(int)fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    return data;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                fs.Close();
            }
        }

        public static void SaveFileData(string fn, byte[] data)
        {
            string dir = Path.GetDirectoryName(fn);
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dir);
            if (!dirinfo.Exists)
                dirinfo.Create();
            FileStream fs = new FileStream(fn, FileMode.Create);
            try
            {
                fs.Write(data, 0, data.Length);
            }
            finally
            {
                fs.Close();
            }
        }

        public static Dictionary<string, string> GetDictionaryFromFile(string fn)
        {
            byte[] data = GetFileData(fn);
            if (data != null)
            {
                ByteReader br = new ByteReader(data);
                return br.ReadDictionary();
            }
            return null;
        }

        public static void SaveDictionary(string fn, Dictionary<string, string> dic)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string k in dic.Keys)
            {
                string v = dic[k];
                sb.Append(string.Format("{0}={1}\r\n", k, v));
            }
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(sb.ToString());
            SaveFileData(fn, data);

        }

        /// <summary>
        /// 绝对路径->相对路径
        /// </summary>
        public static string Absolute2Relativity(string path)
        {
            string temp = path.Substring(path.IndexOf("Assets"));
            temp = temp.Replace('\\', '/');
            return temp;
        }

        /// <summary>
        /// 相对路径->绝对路径
        /// </summary>
        public static string Relativity2Absolute(string path)
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
            path = files[0].FullName.ToString();
            return path;
        }


    }

}