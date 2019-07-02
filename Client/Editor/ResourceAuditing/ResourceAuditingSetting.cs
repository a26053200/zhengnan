using System;
using UnityEngine;
using System.Collections.Generic;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/7/1 0:10:11</para>
    /// </summary> 
    public class ResourceAuditingSetting : ScriptableObject
    {
        public static ResourceAuditingSetting s_Instance;

        public static ResourceAuditingSetting GetIntance()
        {
            return s_Instance;
        }
        /// <summary>
        /// IOS平台推荐使用贴图格式
        /// </summary>
        public TextuteFormatKey[] Tex_Format_Recommend_IOS;

        /// <summary>
        /// IOS平台禁止使用贴图格式
        /// </summary>
        public TextuteFormatKey[] Tex_Format_Forbid_IOS;

        /// <summary>
        /// Android平台推荐使用贴图格式
        /// </summary>
        public TextuteFormatKey[] Tex_Format_Recommend_Android;

        /// <summary>
        /// Android平台禁止使用贴图格式
        /// </summary>
        public TextuteFormatKey[] Tex_Format_Forbid_Android;

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

        //材质贴图 "*.psd|*.tiff|*.jpg|*.jpeg|*.tga|*.png|*.gif"
        public string[] Texture_FileTypes = new string[] { ".psd", ".tiff", ".jpg", ".tga", ".png", ".gif", ".tif", ".dds" };

        public string[] Forbid_Texture_FileTypes = new string[] { ".psd", ".dds" };
        //材质球 
        public string[] Material_FileTypes = new string[] { ".mat" };
        //模型文件
        public string[] Model_FileTypes = new string[] { ".fbx", ".obj" };

        //音频方案
        //1.大点的音频: Streaming,不预加载，Vorbis
        //2.小点的音频DecompressOnLoad 不预加载  ，Vorbis

        //声音大小阈值
        public float autioLengthThreshold = 5.0f;

        public AudioClipLoadType audioClipLoadType_Short_Ios;
        public AudioCompressionFormat compressionFormat_Short_Ios;
        public AudioClipLoadType audioClipLoadType_Long_Ios;
        public AudioCompressionFormat compressionFormat_Long_Ios;

        public AudioClipLoadType audioClipLoadType_Short_Android;
        public AudioCompressionFormat compressionFormat_Short_Android;
        public AudioClipLoadType audioClipLoadType_Long_Android;
        public AudioCompressionFormat compressionFormat_Long_Android;

        public string[] Sound_FileTypes = new string[] { ".aif", ".wav", ".mp3", ".ogg" };

    }
}


