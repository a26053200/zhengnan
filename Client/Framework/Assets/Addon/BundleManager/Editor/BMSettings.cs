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
        public string resDir = "Assets/Res/";

        public string lastBuildDate;

        public int zipLevel = 9;

        //lua文件夹
        public List<string> luaFolderList = new List<string>();
        //打包类型
        public BuildType luaBuildType;
        //
        public CompressType luaCompressType;
        //
        public string luaPattern = "*.lua";

        //场景文件夹
        public List<string> scenesFolderList = new List<string>();
        //打包类型
        public BuildType scenesBuildType;             
        //
        public CompressType scenesCompressType;
        //后缀
        public string scenesPattern = "*.unity";

        //普通文件夹
        public List<string> singleFolderList = new List<string>();
        //打包类型
        public BuildType singleBuildType;
        //
        public CompressType singleCompressType;
        //后缀
        public string singlePattern = "*.*";

        //打包文件夹
        public List<string> packFolderList = new List<string>();
        //打包类型
        public BuildType packBuildType;
        //
        public CompressType packCompressType;
        //后缀
        public string packPattern = "*.*";

        //shader文件夹
        public List<string> shaderFolderList = new List<string>();
        //打包类型
        public BuildType shaderBuildType;
        //
        public CompressType shaderCompressType;
        //后缀
        public string shaderPattern = "*.*";

        //后缀
        public string Suffix_Bundle = "bundle";
        //场景文件后缀
        public string Scene_Suffix = "*.unity";
        //忽略后缀文件
        public string Ignore_Suffix = ".meta,.DS_Store";
        //忽略文件夹
        public string Ignore_Folder = ".svn";

        //拥有依赖性的文件后缀名
        public List<string> ownerDependenceSuffixs = new List<string>();

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
    

