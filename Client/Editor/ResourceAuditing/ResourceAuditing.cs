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
        static string Res_Root_Path;
        //标准配置文件路径
        static string Norm_Setting_Path;
        //材质贴图 "*.psd|*.tiff|*.jpg|*.jpeg|*.tga|*.png|*.gif"
        string[] textureFileTypes = new string[] { ".psd", ".tiff", ".jpg", ".tga", ".png", ".gif", ".tif" };
        //材质球 
        string[] materialFileTypes = new string[] { ".mat"};
        //材质球 
        string[] modelFileTypes = new string[] { ".fbx", ".obj" };
        //常量 
        string[] DetailsStrings = { "Textures", "Materials", "Meshes" };
       
        //变量
        Dictionary<string, TextureDetails> allTexDict;
        Dictionary<string, MaterialDetails> allMatDict;
        Dictionary<string, ModelDetails> allModelDict;
        List<string> allAssetsPaths = new List<string>();

        DetailsType currSelectDetailsType;
        bool isSearching = false; //正在搜索

        ResourceTree<TextureDetails> textureTree;
        ResourceTree<MaterialDetails> materialTree;
        ResourceTree<ModelDetails> modelTree;

        Norm norm;
        void ResetView()
        {
            Res_Root_Path = Application.dataPath + "/Res";
            Norm_Setting_Path = Application.dataPath + "/Norm-Setting.txt";
            GetAllAssets();
            FetchAllTextures();

            

            Norm.GetIntance().LoadNorm(Norm_Setting_Path);
            norm = Norm.GetIntance();
        }
        private void OnLostFocus()
        {
            if(!isSearching)
            {
                Close();
            }
        }

        const string Title_Norm = "Norm Setting:";
        const string Title_Tex_Format_Recommend_IOS     = "Rec Format IOS";
        const string Title_Tex_Format_Forbid_IOS = "Fbd Format IOS";
        const string Title_Tex_Format_Recommend_Android = "Rec Format Android";
        const string Title_Tex_Format_Forbid_Android = "Fbd Format Android";
        const string Title_Tex_Max_Size = "Max Texture Size";
        const string Title_Tex_Recommend_Size = "Rec Texture Size";
        const string Title_Mesh_Max_TrisNum = "Max Mesh Tris Num";
        const string Title_Mesh_Recommend_TrisNum = "Rec Mesh Tris Num";
        const string Title_Shader_Forbid = "Fbd Shader";

        const string Button_Default = "Default";
        const string Button_Save = "Save";

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Title_Norm);
            norm.Tex_Format_Recommend_IOS      = EditorGUILayout.TextField(Title_Tex_Format_Recommend_IOS, norm.Tex_Format_Recommend_IOS);
            norm.Tex_Format_Forbid_IOS         = EditorGUILayout.TextField(Title_Tex_Format_Forbid_IOS, norm.Tex_Format_Forbid_IOS);
            norm.Tex_Format_Recommend_Android   = EditorGUILayout.TextField(Title_Tex_Format_Recommend_Android, norm.Tex_Format_Recommend_Android);
            norm.Tex_Format_Forbid_Android     = EditorGUILayout.TextField(Title_Tex_Format_Forbid_Android, norm.Tex_Format_Forbid_Android);
            norm.Tex_Max_Size                 = EditorGUILayout.IntField(Title_Tex_Max_Size, norm.Tex_Max_Size);
            norm.Tex_Recommend_Size            = EditorGUILayout.IntField(Title_Tex_Recommend_Size, norm.Tex_Recommend_Size);
            norm.Mesh_Max_TrisNum              = EditorGUILayout.IntField(Title_Mesh_Max_TrisNum, norm.Mesh_Max_TrisNum);
            norm.Mesh_Recommend_TrisNum        = EditorGUILayout.IntField(Title_Mesh_Recommend_TrisNum, norm.Mesh_Recommend_TrisNum);
            norm.Shader_Forbid                 = EditorGUILayout.TextField(Title_Shader_Forbid, norm.Shader_Forbid);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("","",GUILayout.Width(position.width * 0.618f));
            if (GUILayout.Button(Button_Default))
            {
                GUI.FocusControl(null);
                Repaint();
                norm.ResetToDefault();
            }
            if (GUILayout.Button(Button_Save))
            {
                norm.SaveNorm(Norm_Setting_Path);
            }
            EditorGUILayout.EndHorizontal();

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
                }
            }
            EditorGUILayout.Space();
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
            if (allTexDict == null)
                allTexDict = FetchAllResources<TextureDetails, TextureResource>(textureFileTypes);
            textureTree = new ResourceTree<TextureDetails>(this, allTexDict, allAssetsPaths);
        }
        /// <summary>
        /// 获取所有材质球
        /// </summary>
        void FetchAllMaterials()
        {
            if (allMatDict == null)
                allMatDict = FetchAllResources<MaterialDetails, MaterialResource>(materialFileTypes);
            materialTree = new ResourceTree<MaterialDetails>(this, allMatDict, allAssetsPaths);
        }
        /// <summary>
        /// 获取模型文件,(网格信息和动作)
        /// </summary>
        void FetchAllModels()
        {
            if (allModelDict == null)
                allModelDict = FetchAllResources<ModelDetails, ModelResource>(modelFileTypes);
            modelTree = new ResourceTree<ModelDetails>(this, allModelDict, allAssetsPaths);
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
