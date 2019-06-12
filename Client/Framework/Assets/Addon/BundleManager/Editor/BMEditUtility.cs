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
            path = files[0].FullName.ToString();
            return path;
        }
    }
}
    

