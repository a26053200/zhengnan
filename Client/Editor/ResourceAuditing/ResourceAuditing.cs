using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

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

        void OnGUI()
        {
            //EditorGUI.BeginChangeCheck();

            currSelectDetailsType = (DetailsType)GUILayout.Toolbar((int)currSelectDetailsType, DetailsStrings);
            //ctrlPressed = Event.current.control || Event.current.command;
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
        void ListTextures()
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
                        td.texture = AssetDatabase.LoadAssetAtPath<Texture>(dir + lowerName);
                        td.fileInfo = fileInfo;
                        allTextures.Add(td);
                    }
                }
            }
            else
            {
                //List Header

                //List Body
                scrollerPos = EditorGUILayout.BeginScrollView(scrollerPos, GUILayout.ExpandWidth(true), GUILayout.Height(100));
                {
                    foreach (var td in allTextures)
                    {
                        EditorGUILayout.ObjectField(td.name, td.texture, typeof(Texture), false);
                    }
                }
                EditorGUILayout.EndScrollView();
                
            }
            
        }
    }
}
