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
        const string Title_FileType = "File type";
        const string Formnat_FileType = "File type Forbid:{0}";

        const string Title_ReadAndWrite = "Read & Write";
        const string Formnat_ReadAndWrite = "Read & Write is not enable";

        const string Title_TextureRealSize = "Real Size";
        const string Formnat_TextureRealSize = "Texture Real Size Max is {0}";

       

        private Texture2D texture;
        private TextureImporter textureImporter;
        private int textureSize;
        private int textureSizeLevel;

        private string fileType;
        private int fileLevel;

        private TexturePlatformNorm standalone_setting;
        private TexturePlatformNorm ios_setting;
        private TexturePlatformNorm android_setting;

        public Texture2D Texture
        {
            get { return texture; }
        }
        public override void Optimization(string param)
        {
            string[] patams = param.Split(',');
            if (textureImporter)
            {
                var format = textureImporter.DoesSourceTextureHaveAlpha()
                    ? (TextureImporterFormat) Enum.Parse(typeof(TextureImporterFormat), patams[1])
                    : (TextureImporterFormat) Enum.Parse(typeof(TextureImporterFormat), patams[2]);
                TextureImporterPlatformSettings platformSettings = textureImporter.GetPlatformTextureSettings(patams[0]);
                TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings
                {
                    allowsAlphaSplitting = platformSettings.allowsAlphaSplitting,
                    crunchedCompression = platformSettings.crunchedCompression,
                    maxTextureSize = platformSettings.maxTextureSize,
                    name = platformSettings.name,
                    resizeAlgorithm = platformSettings.resizeAlgorithm,

                    textureCompression = TextureImporterCompression.Compressed,
                    compressionQuality = (int)UnityEditor.TextureCompressionQuality.Normal,
                    overridden = true
                };
                settings.format = format;
                settings.overridden = true;
                textureImporter.isReadable = false;
                textureImporter.SetPlatformTextureSettings(settings);
                textureImporter.SaveAndReimport();
            }
        }
        public override void SetResObj(Object obj)
        {
            resObj = obj;
            texture = obj as Texture2D;
            textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            //format
            standalone_setting = GetPlatformNorm(textureImporter.GetPlatformTextureSettings(EditPlatform.Standalone),new TextureFormatKey[] { }, new TextureFormatKey[] { });
            ios_setting = GetPlatformNorm(textureImporter.GetPlatformTextureSettings(EditPlatform.iPhone), ResourceAuditingSetting.GetIntance().Tex_Format_Recommend_IOS, ResourceAuditingSetting.GetIntance().Tex_Format_Forbid_IOS); 
            android_setting = GetPlatformNorm(textureImporter.GetPlatformTextureSettings(EditPlatform.Android), ResourceAuditingSetting.GetIntance().Tex_Format_Recommend_Android, ResourceAuditingSetting.GetIntance().Tex_Format_Forbid_Android); 
            //maxSize
            textureSize = Mathf.Max(texture.width, texture.height);
            textureSizeLevel = 0;
            if (textureSize >= ResourceAuditingSetting.GetIntance().Tex_Recommend_Size && textureSize < ResourceAuditingSetting.GetIntance().Tex_Max_Size)
            {
                textureSizeLevel = 1;
                warnNum++;
            }
            else if (textureSize >= ResourceAuditingSetting.GetIntance().Tex_Max_Size)
            {
                textureSizeLevel = 2;
                errorNum++;
            }
            string[] fs = ResourceAuditingSetting.GetIntance().Forbid_Texture_FileTypes;
            fileType = Path.GetFileName(path);
            // file type
            for (int i = 0; i < fs.Length; i++)
            {
                if(path.EndsWith(fs[i]))
                {
                    fileLevel = 2;
                    errorNum++;
                    break;
                }
            }
        }

        public override void OnResourceGUI()
        {
            ResUtils.ColorLabelFieldTooltip(Title_FileType, fileType, Formnat_FileType, fileLevel, 150);
            ResUtils.ColorLabelFieldTooltip(Title_ReadAndWrite, textureImporter.isReadable.ToString(), string.Format(Formnat_ReadAndWrite), !textureImporter.isReadable, 150);
            ResUtils.ColorLabelFieldTooltip(Title_TextureRealSize, textureSize.ToString(), string.Format(Formnat_TextureRealSize, ResourceAuditingSetting.GetIntance().Tex_Max_Size), textureSizeLevel, 150);
            EditorGUILayout.BeginVertical();
            DisplayPlatformSetting(standalone_setting);
            DisplayPlatformSetting(ios_setting);
            DisplayPlatformSetting(android_setting);
            EditorGUILayout.EndVertical();
        }

        public TexturePlatformNorm GetPlatformNorm(TextureImporterPlatformSettings setting, TextureFormatKey[] normRecommend, TextureFormatKey[] normForbid)
        {
            TexturePlatformNorm tpn = new TexturePlatformNorm()
            {
                setting = setting,
                normRecommend = normRecommend,
            };
            string format = setting.format.ToString();
            if (isInclude(normRecommend, format))
            {
                tpn.formatLevel = 0;
            }
            else if (isInclude(normForbid, format))
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
                if (maxSize >= ResourceAuditingSetting.GetIntance().Tex_Recommend_Size && maxSize < ResourceAuditingSetting.GetIntance().Tex_Max_Size)
                {
                    tpn.maxSizeLevel = 1;
                    warnNum++;
                }
                else if (maxSize >= ResourceAuditingSetting.GetIntance().Tex_Max_Size)
                {
                    tpn.maxSizeLevel = 2;
                    errorNum++;
                }
            }
            return tpn;
        }

        const string Platform = "Platform:";

        const string Title_Format = "Format:";
        const string Formnat_Format = "Texture Real Size Max is {0}";

        const string Title_MaxSize = "Max Size:";
        const string Formnat_MaxSize = "Max Size is {0}";

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

        private bool isInclude(TextureFormatKey[] formats, string format)
        {
            for (int i = 0; i < formats.Length; i++) 
            {
                if (format.StartsWith(formats[i].ToString()))
                    return true;
            }
            return false;
        }
       
    }

    public class TexturePlatformNorm
    {
        public TextureImporterPlatformSettings setting;

        public TextureFormatKey[] normRecommend;

        public int formatLevel = 1;
        public int maxSizeLevel = 1;
    }
}
