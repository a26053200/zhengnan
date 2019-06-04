using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

namespace ResourceAuditing
{
    public class ResourceAuditing : BaseEditorWnd
    {

        static int MinWidth = 1024;
        static int MinHeight = 768;

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
        List<Texture> texList = new List<Texture>();

        DetailsType currSelectDetailsType;
        bool ctrlPressed = false;
        Vector2 scrollerPos = Vector2.zero;
        void ResetView()
        {

        }
        private void OnLostFocus()
        {
            Close();
        }
        void OnGUI()
        {
            //EditorGUI.BeginChangeCheck();
            //EditorGUI.EndChangeCheck();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                Array arr = Enum.GetValues(typeof(DetailsType));
                for (int i = 0; i < DetailsStrings.Length; i++)
                {
                    if (GUILayout.Button(DetailsStrings[i]))
                    {
                        currSelectDetailsType = (DetailsType)Enum.ToObject(typeof(DetailsType), i);
                        switch (currSelectDetailsType)
                        {
                            case DetailsType.Textures:
                                GetListTextures();
                                break;
                            case DetailsType.Materials:
                                //ListMaterials();
                                break;
                            case DetailsType.Meshes:
                                //ListMeshes();
                                break;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //currSelectDetailsType = (DetailsType)GUILayout.Toolbar((int)currSelectDetailsType, DetailsStrings);
            ////ctrlPressed = Event.current.control || Event.current.command;
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
            //if (GUILayout.Button("T%"))
            //{
            //    GetListTextures();
            //}
            //ListTextures();


        }

        void GetListTextures()
        {
            if (allTextures.Count == 0)
            {
                // PSD, TIFF, JPG, TGA, PNG, GIF
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
                        TextureDetails td = new TextureDetails();
                        string dir = EditorFileUitl.Absolute2Relativity(fileInfo.DirectoryName) + "/";
                        dir = dir.ToLower();
                        td.name = lowerName;
                        td.path = dir + lowerName;
                        td.texture = AssetDatabase.LoadAssetAtPath<Texture>(dir + lowerName);
                        td.fileInfo = fileInfo;
                        allTextures.Add(td);
                    }
                }
            }
        }
        void ListTextures()
        {
            if (allTextures.Count > 0)
            {
                //List Header
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.EndHorizontal();
                //List Body
                scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {

                    //EditorGUILayout.BeginVertical();
                    for (int i = 0; i < allTextures.Count; i++)
                    {
                        TextureDetails td = allTextures[i];
                        BeginFoldout(td);
                    }
                    //EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
            }
        }
        Rect rect64 = new Rect(32,0,100,64);
        Rect rect32 = new Rect(0, 0, 100, 64);
        void BeginFoldout(TextureDetails td)
        {
            if (td.isOpen)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.BeginArea(rect64);
                    td.isOpen = EditorGUILayout.Foldout(td.isOpen, new GUIContent(td.name), td.isClick);
                    GUILayout.EndArea();
                    //EditorGUILayout.LabelField(td.name);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("", td.texture, typeof(Texture), true, GUILayout.Width(64));
                    EditorGUILayout.TextField("", td.path);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                
                EditorGUILayout.BeginHorizontal();
                {
                    td.isOpen = EditorGUILayout.Foldout(td.isOpen, td.name);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("", td.texture, typeof(Texture), true, GUILayout.Width(32), GUILayout.Height(32));
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
    }
}
