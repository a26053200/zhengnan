using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace ResourceAuditing
{
    public enum TextuteFormatKey
    {
        ETC1,
        ETC2,
        PVRTC,
        ASTC,
        DXT,
    }
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/10 22:00:39</para>
    /// </summary> 
    public class Norm
    {
        static Norm s_Instance;

        public static Norm GetIntance()
        {
            if (s_Instance == null)
                s_Instance = new Norm();
            return s_Instance;
        }
        /// <summary>
        /// IOS平台推荐使用贴图格式
        /// </summary>
        public string Tex_Format_Recommend_IOS = "PVRTC";

        /// <summary>
        /// IOS平台禁止使用贴图格式
        /// </summary>
        public string Tex_Format_Forbid_IOS = "ETC,ASTC,DXT";

        /// <summary>
        /// Android平台推荐使用贴图格式
        /// </summary>
        public string Tex_Format_Recommend_Android = "ETC";

        /// <summary>
        /// Android平台禁止使用贴图格式
        /// </summary>
        public string Tex_Format_Forbid_Android = "ASTC,DXT,PVRTC";

        /// <summary>
        /// 贴图最大尺寸
        /// </summary>
        public int Tex_Max_Size = 2048;

        /// <summary>
        /// 贴图推荐尺寸
        /// </summary>
        public int Tex_Recommend_Size = 1024;

        /// <summary>
        /// 禁止使用的Shader
        /// </summary>
        public string Shader_Forbid = "Standard";

        /// <summary>
        /// 模型最大面数
        /// </summary>
        public int Mesh_Max_TrisNum = 3000;

        /// <summary>
        /// 模型推荐面数
        /// </summary>
        public int Mesh_Recommend_TrisNum = 1500;

        Dictionary<string, string> defaultDict;

        public void LoadNorm(string path)
        {
            defaultDict = new Dictionary<string, string>();
            FieldInfo[] fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                defaultDict.Add(field.Name, field.GetValue(this).ToString());
            }

            Dictionary<string, string> normDict = EditorFileUitl.GetDictionaryFromFile(path);
            if (normDict == null)
                normDict = new Dictionary<string, string>();
            else
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    string value = null;
                    if (normDict.TryGetValue(field.Name, out value))
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

        public void SaveNorm(string path)
        {
            Dictionary<string, string> normDict = new Dictionary<string, string>();
            FieldInfo[] fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                normDict.Add(field.Name, field.GetValue(this).ToString());
            }
            EditorFileUitl.SaveDictionary(path, normDict);
        }
    }
}
    

