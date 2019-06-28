using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;
using UnityEditor;

namespace ResourceAuditing
{
    public class TextureDetails : ResourceDetail
    {
        public string path;
        public int hashCode;

        public TextureDetails(string path, string name) :base(path, name)
        {
            resources = new List<Resource>();
        }
        
    }

    

    public class TextureResource : Resource
    {
        const string Title_ReadAndWrite = "Read & Write";
        const string Formnat_ReadAndWrite = "Read & Write is not enable";

        const string Title_TextureRealSize = "Texture Real Size";
        const string Formnat_TextureRealSize = "Texture Real Size Max is %d";

       

        private Texture2D texture;
        private TextureImporter textureImporter;
        private int textureSize;
        private int textureSizeLevel;

        private TexturePlatformNorm standalone_setting;
        private TexturePlatformNorm ios_setting;
        private TexturePlatformNorm android_setting;

        public Texture2D Texture
        {
            get { return texture; }
        }

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            texture = obj as Texture2D;
            textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            //format
            standalone_setting = GetPlatformNorm(textureImporter.GetPlatformTextureSettings(EditPlatform.Standalone),"","");
            ios_setting = GetPlatformNorm(textureImporter.GetPlatformTextureSettings(EditPlatform.iPhone), Norm.GetIntance().Tex_Format_Recommend_IOS, Norm.GetIntance().Tex_Format_Forbid_IOS); 
            android_setting = GetPlatformNorm(textureImporter.GetPlatformTextureSettings(EditPlatform.Android), Norm.GetIntance().Tex_Format_Recommend_Android, Norm.GetIntance().Tex_Format_Forbid_Android); 
            //maxSize
            textureSize = Mathf.Max(texture.width, texture.height);
            textureSizeLevel = 0;
            if (textureSize >= Norm.GetIntance().Tex_Recommend_Size && textureSize < Norm.GetIntance().Tex_Max_Size)
            {
                textureSizeLevel = 1;
                warnNum++;
            }
            else if (textureSize >= Norm.GetIntance().Tex_Max_Size)
            {
                textureSizeLevel = 2;
                errorNum++;
            }
        }

        public override void OnResourceGUI()
        {
            ResUtils.ColorLabelFieldTooltip(Title_ReadAndWrite, textureImporter.isReadable.ToString(), string.Format(Formnat_ReadAndWrite), !textureImporter.isReadable, 150);
            ResUtils.ColorLabelFieldTooltip(Title_TextureRealSize, textureSize.ToString(), string.Format(Formnat_TextureRealSize, Norm.GetIntance().Tex_Max_Size), textureSizeLevel, 150);
            EditorGUILayout.BeginVertical();
            DisplayPlatformSetting(standalone_setting);
            DisplayPlatformSetting(ios_setting);
            DisplayPlatformSetting(android_setting);
            EditorGUILayout.EndVertical();
        }

        public TexturePlatformNorm GetPlatformNorm(TextureImporterPlatformSettings setting, string normRecommend, string normForbid)
        {
            TexturePlatformNorm tpn = new TexturePlatformNorm()
            {
                setting = setting,
                normRecommend = normRecommend,
            };
            string[] recommend_formats = normRecommend.Split(',');
            string[] forbid_formats = normForbid.Split(',');
            //format
            string format = setting.format.ToString();
            if (isInclude(recommend_formats, format))
            {
                tpn.formatLevel = 0;
            }
            else if (isInclude(forbid_formats, format))
            {
                tpn.formatLevel = 2;
                errorNum++;
            }
            else
            {
                tpn.formatLevel = 1;
                warnNum++;
            }

            //maxSize
            int maxSize = setting.maxTextureSize;
            if (maxSize <= textureSize)
            {
                if (maxSize >= Norm.GetIntance().Tex_Recommend_Size && maxSize < Norm.GetIntance().Tex_Max_Size)
                {
                    tpn.maxSizeLevel = 1;
                    warnNum++;
                }
                else if (maxSize >= Norm.GetIntance().Tex_Max_Size)
                {
                    tpn.maxSizeLevel = 2;
                    errorNum++;
                }
            }
            return tpn;
        }

        const string Platform = "Platform";

        const string Title_Format = "Texture Format";
        const string Formnat_Format = "Texture Real Size Max is %d";

        const string Title_MaxSize = "Max Texture Size";
        const string Formnat_MaxSize = "Max Size is %d";

        public void DisplayPlatformSetting(TexturePlatformNorm tpn)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Platform, tpn.setting.name);
            //format
            string format = tpn.setting.format.ToString();
            ResUtils.ColorLabelFieldTooltip(Title_Format, format, string.Format(Formnat_Format, tpn.normRecommend), tpn.formatLevel);
            ResUtils.ColorLabelFieldTooltip(Title_MaxSize, tpn.setting.maxTextureSize.ToString(), string.Format(Formnat_MaxSize, tpn.setting.maxTextureSize), tpn.maxSizeLevel);
            EditorGUILayout.EndHorizontal();
        }

        private bool isInclude(string[] formats, string format)
        {
            for (int i = 0; i < formats.Length; i++)
            {
                if (format.StartsWith(formats[i]))
                    return true;
            }
            return false;
        }
       
    }

    public class TexturePlatformNorm
    {
        public TextureImporterPlatformSettings setting;

        public string normRecommend;

        public int formatLevel = 1;
        public int maxSizeLevel = 1;
    }
}
