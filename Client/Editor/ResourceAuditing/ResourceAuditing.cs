using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;
using UnityEditor.IMGUI.Controls;

namespace ResourceAuditing
{
    public class ResourceAuditing : BaseEditorWnd
    {

        static int MinWidth = 600;
        static int MinHeight = 400;

        [MenuItem("Window/Resource Auditing")]
        static void Init()
        {
            ResourceAuditing window = (ResourceAuditing)EditorWindow.GetWindow(typeof(ResourceAuditing));
            window.ResetView();
            window.minSize = new Vector2(MinWidth, MinHeight);
        }
        enum DetailsType
        {
            Textures, Materials, Meshes, Audios, Missing
        };

        //资源目录
        static string Res_Root_Path;
        //标准配置文件路径
        static string Resource_Auditing_Setting_Path;
        //常量 
        static string[] DetailsStrings = { "Textures", "Materials", "Meshes", "Audios" };

        //变量
        Dictionary<string, TextureDetails> allTexDict;
        Dictionary<string, MaterialDetails> allMatDict;
        Dictionary<string, ModelDetails> allModelDict;
        Dictionary<string, AudioDetails> allAudioDict;
        List<string> allAssetsPaths = new List<string>();

        DetailsType currSelectDetailsType;
        bool isSearching = false; //正在搜索

        ResourceTree<TextureDetails> textureTree;
        ResourceTree<MaterialDetails> materialTree;
        ResourceTree<ModelDetails> modelTree;
        ResourceTree<AudioDetails> audioTree;

        bool isCloseWhenLostFocus;

        ResourceAuditingSetting setting;

        void ResetView()
        {

            Res_Root_Path = Application.dataPath + "/Res";
            Resource_Auditing_Setting_Path = "Assets/Res/ResourceAuditingSetting.asset";
            
            if (!File.Exists(Resource_Auditing_Setting_Path))
                ResUtils.CreateAsset<ResourceAuditingSetting>(Resource_Auditing_Setting_Path);
            setting = AssetDatabase.LoadAssetAtPath<ResourceAuditingSetting>(Resource_Auditing_Setting_Path);
            ResourceAuditingSetting.s_Instance = setting;

            GetAllAssets();
            FetchAllTextures();
        }

        private void OnLostFocus()
        {
            if(!isSearching && isCloseWhenLostFocus)
            {
                Close();
            }
        }

        //const string Title_Norm = "Norm Setting:";
        //const string Title_Tex_Format_Recommend_IOS     = "Rec Format IOS";
        //const string Title_Tex_Format_Forbid_IOS = "Fbd Format IOS";
        //const string Title_Tex_Format_Recommend_Android = "Rec Format Android";
        //const string Title_Tex_Format_Forbid_Android = "Fbd Format Android";
        //const string Title_Tex_Max_Size = "Max Texture Size";
        //const string Title_Tex_Recommend_Size = "Rec Texture Size";
        //const string Title_Mesh_Max_TrisNum = "Max Mesh Tris Num";
        //const string Title_Mesh_Recommend_TrisNum = "Rec Mesh Tris Num";
        //const string Title_Shader_Forbid = "Fbd Shader";

        //const string Button_Default = "Default";
        //const string Button_Save = "Save";

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            DetailsType old = currSelectDetailsType;
            currSelectDetailsType = (DetailsType)GUILayout.Toolbar((int)currSelectDetailsType, DetailsStrings);
            if(EditorGUI.EndChangeCheck() && old != currSelectDetailsType)
            {
                switch (currSelectDetailsType)
                {
                    case DetailsType.Textures:
                        FetchAllTextures();
                        break;
                    case DetailsType.Materials:
                        FetchAllMaterials();
                        break;
                    case DetailsType.Meshes:
                        FetchAllModels();
                        break;
                    case DetailsType.Audios:
                        FetchAllSounds();
                        break;
                }
            }
            EditorGUILayout.Space();
            switch (currSelectDetailsType)
            {
                case DetailsType.Textures:
                    OptimizeTextures();
                    textureTree.OnGUI();
                    break;
                case DetailsType.Materials:
                    materialTree.OnGUI();
                    break;
                case DetailsType.Meshes:
                    modelTree.OnGUI();
                    break;
                case DetailsType.Audios:
                    audioTree.OnGUI();
                    break;
            }
        }

        private TextureImporterFormat _iosTif = TextureImporterFormat.ASTC_RGB_6x6;
        private TextureImporterFormat _iosTif_a = TextureImporterFormat.ASTC_RGBA_6x6;
        private TextureImporterFormat _androidTif = TextureImporterFormat.ETC2_RGB4;
        private TextureImporterFormat _androidTif_a = TextureImporterFormat.ETC2_RGBA8;
        void OptimizeTextures()
        {
            if (allTexDict != null)
            {
                EditorGUILayout.BeginHorizontal();
                _iosTif = (TextureImporterFormat)EditorGUILayout.EnumFlagsField("IOS:", _iosTif);
                _iosTif_a = (TextureImporterFormat)EditorGUILayout.EnumFlagsField("Alpha:", _iosTif_a);
                if(GUILayout.Button("Optimize"))
                    DoOptimizeTextures(EditPlatform.iPhone, _iosTif, _iosTif_a);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                _androidTif = (TextureImporterFormat)EditorGUILayout.EnumFlagsField("Android:", _androidTif);
                _androidTif_a = (TextureImporterFormat)EditorGUILayout.EnumFlagsField("Alpha:", _androidTif_a);
                if(GUILayout.Button("Optimize"))
                {
                    DoOptimizeTextures(EditPlatform.Android, _androidTif,_androidTif_a);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void DoOptimizeTextures(string platform, TextureImporterFormat tif, TextureImporterFormat tif_a)
        {
            foreach (var detail in allTexDict.Values)
            {
                for (int i = 0; i < detail.resources.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Optimize...", detail.resources[i].path, (float)i + 1.0f / (float)detail.resources.Count);
                    detail.resources[i].Optimization(platform + "," + tif+ "," + tif_a);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        
        /// <summary>
        /// 获取所有贴图
        /// </summary>
        void FetchAllTextures()
        {
            if (allTexDict == null)
                allTexDict = FetchAllResources<TextureDetails, TextureResource>(setting.Texture_FileTypes);
            textureTree = new ResourceTree<TextureDetails>(this, allTexDict, allAssetsPaths);
        }
        /// <summary>
        /// 获取所有材质球
        /// </summary>
        void FetchAllMaterials()
        {
            if (allMatDict == null)
                allMatDict = FetchAllResources<MaterialDetails, MaterialResource>(setting.Material_FileTypes);
            materialTree = new ResourceTree<MaterialDetails>(this, allMatDict, allAssetsPaths);
        }
        /// <summary>
        /// 获取模型文件,(网格信息和动作)
        /// </summary>
        void FetchAllModels()
        {
            if (allModelDict == null)
                allModelDict = FetchAllResources<ModelDetails, ModelResource>(setting.Model_FileTypes);
            modelTree = new ResourceTree<ModelDetails>(this, allModelDict, allAssetsPaths);
        }

        /// <summary>
        /// 获取音频文件,(网格信息和动作)
        /// </summary>
        void FetchAllSounds()
        {
            if (allAudioDict == null)
                allAudioDict = FetchAllResources<AudioDetails, AudioResource>(setting.Sound_FileTypes);
            audioTree = new ResourceTree<AudioDetails>(this, allAudioDict, allAssetsPaths);
        }

        #region 获取所有的asset
        void GetAllAssets()
        {
            allAssetsPaths = new List<string>();
            string[] _tempList = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < _tempList.Length; i++)
            {
                if (_tempList[i].Contains("Assets"))
                {
                    if (!AssetDatabase.IsValidFolder(_tempList[i]))
                    {
                        if (_tempList[i].Contains("Assets/Res"))
                        {
                            allAssetsPaths.Add(_tempList[i]);
                        }
                    }
                }
            }
        }
        #endregion



        #region 获取所有制定的资源
        Dictionary<string, RD> FetchAllResources<RD, R>(string[] fileTypes) where RD : ResourceDetail where R : Resource
        {
            isSearching = true;
            Dictionary<string, RD> allDict = new Dictionary<string, RD>();
            FileInfo[] resFiles = EditorFileUitl.GetAllFiles(Res_Root_Path, "*.*");
            for (int i = 0; i < resFiles.Length; i++)
            {
                FileInfo fileInfo = resFiles[i];
                string lowerName = fileInfo.Name.ToLower();
                bool find = false;
                for (int j = 0; j < fileTypes.Length; j++)
                {
                    if (lowerName.EndsWith(fileTypes[j]))
                    {
                        find = true;
                        break;
                    }
                }
                if (find)
                {
                    string dir = EditorFileUitl.Absolute2Relativity(fileInfo.DirectoryName) + "/";
                    string path = dir + fileInfo.Name;
                    EditorUtility.DisplayProgressBar("Resource Searching...", path, (float)i + 1.0f / (float)resFiles.Length);
                    string md5 = ResUtils.GetFileMD5(path);
                    RD rd = null;
                    if (!allDict.TryGetValue(md5, out rd))
                    {
                        
                        rd = (RD) System.Activator.CreateInstance(typeof(RD), md5, fileInfo.Name);
                        allDict.Add(md5, rd);
                    }
                    R r = (R)System.Activator.CreateInstance(typeof(R));
                    r.name = fileInfo.Name;
                    r.path = dir + fileInfo.Name;
                    r.SetResObj(AssetDatabase.LoadAssetAtPath<Object>(path));
                    r.fileInfo = fileInfo;
                    r.hashCode = r.resObj.GetHashCode();
                    rd.resources.Add(r);
                }
            }
            EditorUtility.ClearProgressBar();
            isSearching = false;
            return allDict;
        }
        #endregion
    }
}
