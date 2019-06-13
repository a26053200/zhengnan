using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>
        /**
        Assets/Res/Atlas	图集目录
        Assets/Res/Sprites/Atlas 精灵图集目录
        Assets/Res/Sprites/Prefabs 精灵预制件目录
        Assets/Res/Prefabs 预制目录
        Assets/Res/Effects 特效目录
        Assets/Res/Materias 材质目录
        Assets/Res/Textures 贴图目录
        Assets/Res/Models 模型目录
        Assets/Res/Shaders 着色器目录
        Assets/Res/Scenes 场景目录
        */
    /// </para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/13 22:13:32</para>
    /// </summary> 
    public class BMSettings
    {
        static BMSettings s_Instance;

        public static BMSettings GetIntance()
        {
            if (s_Instance == null)
                s_Instance = new BMSettings();
            return s_Instance;
        }

        private string BMSettings_Path;

        public BMSettings()
        {
            BMSettings_Path = Application.dataPath + "Res/BMSettings.txt";
        }
        /// <summary>
        /// 图集目录
        /// </summary>
        public string Folder_Atlas = "Assets/Res/Atlas";

        /// <summary>
        /// 精灵图集目录
        /// </summary>
        public string Folder_Sprites_Atlas = "Assets/Res/Sprites/Atlas";

        /// <summary>
        /// 精灵预制件目录
        /// </summary>
        public string Folder_Sprites_Prefabs = "Assets/Res/Sprites/Prefabs";

        /// <summary>
        /// 预制目录
        /// </summary>
        public string Folder_Prefabs = "Assets/Res/Prefabs";

        /// <summary>
        /// 特效目录
        /// </summary>
        public string Folder_Effects = "Assets/Res/Effects";

        /// <summary>
        /// 材质目录
        /// </summary>
        public string Folder_Materias = "Assets/Res/Materias";

        /// <summary>
        /// 贴图目录
        /// </summary>
        public string Folder_Textures = "Assets/Res/Textures";

        /// <summary>
        /// 模型目录
        /// </summary>
        public string Folder_Models = "Assets/Res/Models";

        /// <summary>
        /// 着色器目录
        /// </summary>
        public string Folder_Shaders = "Assets/Res/Shaders";

        /// <summary>
        /// 场景目录
        /// </summary>
        public string Folder_Scenes = "Assets/Res/Scenes";

        /// 资源根目录
        public string ResFolder = "Assets/Res";
        
        //后缀
        public string Suffix_Bundle = ".bundle";
        //忽略后缀文件
        public string Ignore_Suffix = ".meta,.DS_Store";
        //忽略文件夹
        public string Ignore_Folder = ".svn";

        Dictionary<string, string> defaultDict;

        public void LoadSettings()
        {
            defaultDict = new Dictionary<string, string>();
            FieldInfo[] fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                defaultDict.Add(field.Name, field.GetValue(this).ToString());
            }

            Dictionary<string, string> settingsDict = BMEditUtility.GetDictionaryFromFile(BMSettings_Path);
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
            FieldInfo[] fields = GetType().GetFields();
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
            FieldInfo[] fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                settingsDict.Add(field.Name, field.GetValue(this).ToString());
            }
            BMEditUtility.SaveDictionary(BMSettings_Path, settingsDict);
        }
    }
}
    

