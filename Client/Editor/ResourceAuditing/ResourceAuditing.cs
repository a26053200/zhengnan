using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;

namespace ResourceAuditing
{
    public class ResourceAuditing : BaseEditorWnd
    {

        static int MinWidth = 400;
        static int MinHeight = 600;

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
        string[] textureTypes = new string[] { ".psd", ".tiff", ".jpg", ".tga", ".png", ".gif" };
        //常量 
        string[] DetailsStrings = { "Textures", "Materials", "Meshes" };
       
        //变量
        List<TextureDetails> allTextures = new List<TextureDetails>();
        Dictionary<string, TextureDetails> allTexDict = new Dictionary<string, TextureDetails>();
        List<Texture> texList = new List<Texture>();
        List<string> allAssetsPaths = new List<string>();

        DetailsType currSelectDetailsType;
        bool ctrlPressed = false;
        Vector2 scrollerPos = Vector2.zero;
        void ResetView()
        {
            GetAllAssets();
            FetchAllTextures();
        }
        private void OnLostFocus()
        {
            //Close();
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
                        //ListMaterials();
                        break;
                    case DetailsType.Meshes:
                        //ListMeshes();
                        break;
                }
            }
            switch (currSelectDetailsType)
            {
                case DetailsType.Textures:
                    ListTextures();
                    break;
                case DetailsType.Materials:
                    //ListMaterials();
                    break;
                case DetailsType.Meshes:
                    //ListMeshes();
                    break;
            }
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

        /// <summary>
        /// 获取所有贴图
        /// </summary>
        void FetchAllTextures()
        {
            if (allTextures.Count == 0)
            {
                allTexDict.Clear();
                // 获取所有贴图
                FileInfo[] texFiles = EditorFileUitl.GetAllFiles(Res_Root_Path, "*.*");
                foreach (var fileInfo in texFiles)
                {
                    string lowerName = fileInfo.Name.ToLower();
                    bool find = false;
                    for (int i = 0; i < textureTypes.Length; i++)
                    {
                        if (lowerName.EndsWith(textureTypes[i]))
                        {
                            find = true;
                            break;
                        }
                    }
                    if (find)
                    {
                        string dir = EditorFileUitl.Absolute2Relativity(fileInfo.DirectoryName) + "/";
                        string path = dir + fileInfo.Name;
                        Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
                        string md5 = ResUtils.GetFileMD5(path);
                        TextureDetails td = null;
                        if(!allTexDict.TryGetValue(md5, out td))
                        {
                            td = new TextureDetails();
                            td.md5 = md5;
                            allTexDict.Add(md5, td);
                        }
                        TextureResource tr = new TextureResource()
                        {
                            name = fileInfo.Name,
                            path = dir + fileInfo.Name,
                            texture = AssetDatabase.LoadAssetAtPath<Texture>(path),
                            fileInfo = fileInfo,
                        };
                        td.resources.Add(tr);

                        allTextures.Add(td);
                    }
                }
            }
        }
        void ListTextures()
        {
            if (allTexDict.Count > 0)
            {
                //List Header
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.EndHorizontal();
                //List Body
                scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {

                    //EditorGUILayout.BeginVertical();
                    foreach (var td in allTexDict.Values)
                    {
                        BeginFoldout(td);
                    }
                    //EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
            }
        }
        void BeginFoldout(TextureDetails td)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("", td.resources[0].texture, typeof(Texture), false, GUILayout.Width(64));
                EditorGUI.EndDisabledGroup();
                td.isOpen = EditorGUILayout.ToggleLeft("", td.isOpen, GUILayout.Width(32));
                if (td.isOpen)
                {
                    //td.isOpen = EditorGUILayout.Foldout(td.isOpen, new GUIContent("-------" + td.name), td.isClick);
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("md5", td.md5);
                        for (int i = 0; i < td.resources.Count; i++)
                        {
                            var tr = td.resources[i];
                            EditorGUILayout.LabelField("", tr.path);
                            EditorGUILayout.BeginHorizontal();
                            {
                                //EditorGUILayout.BeginVertical();
                                {
                                    //EditorGUI.BeginDisabledGroup(true);
                                    //EditorGUILayout.ObjectField("", tr.texture, typeof(Texture), false, GUILayout.Width(64));
                                    //EditorGUI.EndDisabledGroup();
                                }
                                //EditorGUILayout.EndVertical();
                                //Object obj = AssetDatabase.LoadAssetAtPath(tr.path, typeof(Object));
                                //EditorGUILayout.ObjectField("", obj, typeof(Object), false);
                                //使用过的
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.BeginVertical();
                                {
                                    string[] _TempArray = GetUseAssetPaths(tr.path);
                                    EditorGUI.indentLevel++;
                                    for (int j = 0; j < _TempArray.Length; j++)
                                    {
                                        Object obj = AssetDatabase.LoadAssetAtPath(_TempArray[j], typeof(Object));
                                        if (!AssetDatabase.IsSubAsset(obj))
                                        {//排除FBX 内部引用
                                            EditorGUILayout.ObjectField("", obj, typeof(Object), false);
                                        }
                                    }
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUI.EndDisabledGroup();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    {
                        var tr = td.resources[0];
                        //EditorGUI.BeginDisabledGroup();
                        //GUILayout.DelayedIntField(rect);
                        //GUILayout.Label(AssetDatabase.GetCachedIcon(td.resources[0].path), new GUILayoutOption[] { GUILayout.Width(30), GUILayout.Height(30) });
                        EditorGUILayout.LabelField("md5", td.md5);
                        //EditorGUILayout.ObjectField("", td.texture, typeof(Texture), true, GUILayout.Width(32), GUILayout.Height(32));
                        //EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        #region 获取其他引用Assets的路径
        string[] GetUseAssetPaths(string _AssetPath)
        {
            List<string> _AssetPaths = new List<string>();
            //使用GUID作为判断标准
            string _AssetGUID = AssetDatabase.AssetPathToGUID(_AssetPath);
            //遍历所有Assets
            for (int i = 0; i < allAssetsPaths.Count; i++)
            {
                if (allAssetsPaths[i] == _AssetPath)
                    continue;

                string[] _OtherPaths = AssetDatabase.GetDependencies(allAssetsPaths[i]);
                if (_OtherPaths.Length > 1)
                {
                    for (int j = 0; j < _OtherPaths.Length; j++)
                    {
                        string _OtherGUID = AssetDatabase.AssetPathToGUID(_OtherPaths[j]);
                        if (_AssetGUID == _OtherGUID)
                        {
                            _AssetPaths.Add(allAssetsPaths[i]);
                        }
                    }
                }
            }
            return _AssetPaths.ToArray();
        }
        #endregion
    }
}
