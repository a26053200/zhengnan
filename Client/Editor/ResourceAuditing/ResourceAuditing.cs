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
            Textures, Materials, Meshes, Missing
        };

        //资源目录
        static string Res_Root_Path = Application.dataPath + "/Res";
        //材质贴图 "*.psd|*.tiff|*.jpg|*.jpeg|*.tga|*.png|*.gif"
        string[] textureFileTypes = new string[] { ".psd", ".tiff", ".jpg", ".tga", ".png", ".gif" };
        //材质球 
        string[] materialFileTypes = new string[] { ".mat"};
        //材质球 
        string[] modelFileTypes = new string[] { ".fbx", ".obj" };
        //常量 
        string[] DetailsStrings = { "Textures", "Materials", "Meshes" };
       
        //变量
        Dictionary<string, TextureDetails> allTexDict = new Dictionary<string, TextureDetails>();
        Dictionary<string, MaterialDetails> allMatDict = new Dictionary<string, MaterialDetails>();
        Dictionary<string, ModelDetails> allModelDict = new Dictionary<string, ModelDetails>();
        List<string> allAssetsPaths = new List<string>();

        DetailsType currSelectDetailsType;
        bool isSearching = false; //正在搜索

        ResourceTree<TextureDetails> textureTree;
        ResourceTree<MaterialDetails> materialTree;
        ResourceTree<ModelDetails> modelTree;

        void ResetView()
        {
            GetAllAssets();
            FetchAllTextures();

            textureTree = new ResourceTree<TextureDetails>(allTexDict, allAssetsPaths);
        }
        private void OnLostFocus()
        {
            if(!isSearching)
            {
                Close();
            }
        }
        void OnGUI()
        {

            EditorGUI.BeginChangeCheck();
            currSelectDetailsType = (DetailsType)GUILayout.Toolbar((int)currSelectDetailsType, DetailsStrings);
            if(EditorGUI.EndChangeCheck())
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
                }
            }
            switch (currSelectDetailsType)
            {
                case DetailsType.Textures:
                    textureTree.OnGUI();
                    break;
                case DetailsType.Materials:
                    materialTree.OnGUI();
                    break;
                case DetailsType.Meshes:
                    modelTree.OnGUI();
                    break;
            }
        }


        /// <summary>
        /// 获取所有贴图
        /// </summary>
        void FetchAllTextures()
        {
            allTexDict = FetchAllResources<TextureDetails, TextureResource>(textureFileTypes);
        }
        /// <summary>
        /// 获取所有材质球
        /// </summary>
        void FetchAllMaterials()
        {
            allMatDict = FetchAllResources<MaterialDetails, MaterialResource>(materialFileTypes);
            materialTree = new ResourceTree<MaterialDetails>(allMatDict, allAssetsPaths);
        }
        /// <summary>
        /// 获取模型文件,(网格信息和动作)
        /// </summary>
        void FetchAllModels()
        {
            allModelDict = FetchAllResources<ModelDetails, ModelResource>(modelFileTypes);
            modelTree = new ResourceTree<ModelDetails>(allModelDict, allAssetsPaths);
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
                        rd.MD5 = md5;
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
