using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/7/1 22:16:37</para>
    /// </summary> 
    public class AudioDetails : ResourceDetail
    {
        public string path;
        public int hashCode;

        public AudioDetails(string path, string name) : base(path, name)
        {
            resources = new List<Resource>();
        }
    }

    public class AudioResource : Resource
    {
        private AudioClip clip;
        private AudioImporter audioImporter;
        private string autioType;
        private ResourceAuditingSetting setting;

        private AudioPlatformNorm standalone_setting;
        private AudioPlatformNorm ios_setting;
        private AudioPlatformNorm android_setting;

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            clip = obj as AudioClip;
            audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            setting = ResourceAuditingSetting.GetIntance();
            if (clip.length > setting.autioLengthThreshold)
                autioType = "Long";
            else
                autioType = "Short";
            //standalone_setting = GetPlatformSetting(audioImporter.GetOverrideSampleSettings(EditPlatform.Standalone), new TextuteFormatKey[] { }, new TextuteFormatKey[] { });
            ios_setting = GetPlatformSetting(audioImporter.GetOverrideSampleSettings(EditPlatform.iPhone), EditPlatform.iPhone,
                setting.audioClipLoadType_Short_Ios, setting.compressionFormat_Short_Ios,
                setting.audioClipLoadType_Long_Ios, setting.compressionFormat_Long_Ios);
            android_setting = GetPlatformSetting(audioImporter.GetOverrideSampleSettings(EditPlatform.Android), EditPlatform.Android,
                setting.audioClipLoadType_Short_Android, setting.compressionFormat_Short_Android,
                setting.audioClipLoadType_Long_Android, setting.compressionFormat_Long_Android);
        }

        const string Title_AudioLength = "Audio Length({0})";
        const string Content_AudioLength = "length:{0}  audio type:{1}";
        const string Formnat_AudioLength = "Less than {0} is short audio. otherwise is long audio";

        const string Title_Preload = "Preload";
        const string Formnat_Preload = "Recommend Preload is 'False'";

        const string Title_loadInBackground = "Load In Bg";
        const string Formnat_loadInBackground = "Recommend Load In Bg is 'True'";

        public override void OnResourceGUI()
        {
            
            ResUtils.ColorLabelFieldTooltip(string.Format(Title_AudioLength, setting.autioLengthThreshold),
                string.Format(Content_AudioLength, clip.length, autioType), 
                string.Format(Formnat_AudioLength, setting.autioLengthThreshold), 0, 200);
            ResUtils.ColorLabelFieldTooltip(Title_Preload, clip.preloadAudioData.ToString(), Formnat_Preload, !clip.preloadAudioData, 200);
            ResUtils.ColorLabelFieldTooltip(Title_loadInBackground, clip.loadInBackground.ToString(), Formnat_loadInBackground, clip.loadInBackground, 200);
            EditorGUILayout.BeginVertical();
            DisplayPlatformSetting(ios_setting);
            DisplayPlatformSetting(android_setting);
            EditorGUILayout.EndVertical();
        }

        const string Platform = "Platform:";

        const string Title_LoadType = "Load Tpye:";
        const string Formnat_LoadType = "Recommend Load Tpye is {0}";

        const string Title_CompressionFormat = "CompressFormat:";
        const string Formnat_CompressionFormat = "Recommend CompressionFormat is {0}";

        public void DisplayPlatformSetting(AudioPlatformNorm apn)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Platform, apn.platform);
            ResUtils.ColorLabelFieldTooltip(Title_LoadType, apn.setting.loadType.ToString(), string.Format(Formnat_LoadType, apn.recommendloadType), apn.loadTypeLevel);
            ResUtils.ColorLabelFieldTooltip(Title_CompressionFormat, apn.setting.compressionFormat.ToString(), string.Format(Formnat_CompressionFormat, apn.recommendCompressionFormat), apn.compressionFormatLevel);
            EditorGUILayout.EndHorizontal();
        }

        public AudioPlatformNorm GetPlatformSetting(AudioImporterSampleSettings platformSetting, string platform, 
            AudioClipLoadType audioClipLoadType_Short, AudioCompressionFormat compressionFormat_Short, 
            AudioClipLoadType audioClipLoadType_Long, AudioCompressionFormat compressionFormat_Long)
        {
            AudioPlatformNorm apn = new AudioPlatformNorm()
            {
                platform = platform,
                setting = platformSetting,
            };
            if (autioType == "Long")
            {
                apn.recommendloadType = audioClipLoadType_Long;
                apn.recommendCompressionFormat = compressionFormat_Long;
                if (clip.loadType != audioClipLoadType_Long)
                {
                    apn.loadTypeLevel = 2;
                    errorNum++;
                }
                else
                {
                    apn.loadTypeLevel = 0;
                }
                if (audioImporter.defaultSampleSettings.compressionFormat != compressionFormat_Long)
                {
                    apn.compressionFormatLevel = 2;
                    errorNum++;
                }
                else
                {
                    apn.compressionFormatLevel = 0;
                }
            }
            else
            {
                apn.recommendloadType = audioClipLoadType_Short;
                apn.recommendCompressionFormat = compressionFormat_Short;
                if (clip.loadType != audioClipLoadType_Short)
                {
                    apn.loadTypeLevel = 2;
                    errorNum++;
                }
                else
                {
                    apn.loadTypeLevel = 0;
                }
                if (audioImporter.defaultSampleSettings.compressionFormat != compressionFormat_Short)
                {
                    apn.compressionFormatLevel = 2;
                    errorNum++;
                }
                else
                {
                    apn.compressionFormatLevel = 0;
                }
            }
            
            return apn;
        }

        public class AudioPlatformNorm
        {
            public string platform;
            public AudioImporterSampleSettings setting;

            private string autioType;
            public int loadTypeLevel = 0;
            public int compressionFormatLevel = 0;
            public AudioClipLoadType recommendloadType;
            public AudioCompressionFormat recommendCompressionFormat;
        }
    }
}


