using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/13 22:13:32</para>
    /// </summary> 
    public class BMSettings : ScriptableObject
    {
        //场景文件夹
        public List<string> scenesFolderList = new List<string>();
        //
        public CompressType scenesCompressType;
        //后缀
        public string scenesPattern = "*.unity";

        //普通文件夹
        public List<string> bundleFolderList = new List<string>();
        //
        public CompressType bundleCompressType;
        //后缀
        public string bundlePattern = "*.*";

        //打包文件夹
        public List<string> packFolderList = new List<string>();
        //
        public CompressType packCompressType;
        //后缀
        public string packPattern = "*.*";

        //整包文件夹
        public List<string> completeFolderList = new List<string>();
        //
        public CompressType completeCompressType;
        //后缀
        public string completePattern = "*.*";

        //后缀
        public string Suffix_Bundle = ".bundle";
        //场景文件后缀
        public string Scene_Suffix = "*.unity";
        //忽略后缀文件
        public string Ignore_Suffix = ".meta,.DS_Store";
        //忽略文件夹
        public string Ignore_Folder = ".svn";

        Dictionary<string, string> defaultDict;

        public void LoadSettings()
        {
            defaultDict = new Dictionary<string, string>();
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                defaultDict.Add(field.Name, field.GetValue(this).ToString());
            }

            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            if (settingsDict == null)
                settingsDict = new Dictionary<string, string>();
            else
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    string value = null;
                    if (settingsDict.TryGetValue(field.Name, out value))
                    {
                        if (field.FieldType == typeof(int))
                        {
                            field.SetValue(this, int.Parse(value));
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            field.SetValue(this, value);
                        }
                    }
                }
            }
        }

        public void ResetToDefault()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                string value = null;
                if (defaultDict.TryGetValue(field.Name, out value))
                {
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(this, int.Parse(value));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(this, value);
                    }
                    Debug.Log(field.FieldType + " - " + value);
                }
            }
        }

        public void SaveSettings()
        {
            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                settingsDict.Add(field.Name, field.GetValue(this).ToString());
            }
            //BMEditUtility.SaveDictionary(BMSettings_Path, settingsDict);
        }
    }
}
    

