using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 22:48:38</para>
    /// </summary> 
    public static class BMEditUtility
    {

        /// <summary>
        /// 获取该目录下面所有的文件,包含子目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">文件后缀名  例如: "(*.jpg|*.bmp)"</param>
        /// <returns></returns>
        public static FileInfo[] GetAllFiles(string path, string searchPattern)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles(searchPattern, SearchOption.AllDirectories);
            return files;
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
            path = files[0].DirectoryName;
            return path;
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
    }
}
    

