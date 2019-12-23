using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/7/4 0:19:19</para>
    /// </summary> 
    [CustomEditor(typeof(BMSettings))]
    public class BMSettingsEditor : Editor
    {
        BMSettings settings;
        private GUIStyle newSceneStyle;

        private void OnEnable()
        {
            settings = target as BMSettings;
            newSceneStyle = new GUIStyle();
            newSceneStyle.normal.textColor = Color.green;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Atlas Sprite"))
            {
                GenerateAtlasSprite(false);
            }
            if (GUILayout.Button("Force Generate Atlas Sprite"))
            {
                GenerateAtlasSprite(true);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            //uiSpriteDir
            settings.uiSpriteDir = EditorGUILayout.TextField("UI Sprite", settings.uiSpriteDir);
            if (!string.IsNullOrEmpty(settings.uiSpriteDir))
            {
                if (GUILayout.Button("Update All Packing Tag"))
                {
                    UpdatePackingTag(settings.uiSpriteDir);
                }
            }
            
            EditorGUILayout.Space();
            foreach (var folder in settings.scenesFolderList)
            {
                FileInfo[] sceneFiles = BMEditUtility.GetAllFiles(folder, settings.scenesPattern);
                    
                for (int i = 0; i < sceneFiles.Length; i++)
                {
                    string path = BMEditUtility.Absolute2Relativity(sceneFiles[i].DirectoryName) + "/" + sceneFiles[i].Name; //相对路径
                    int index = settings.scenePaths.IndexOf(path);
                    EditorGUILayout.BeginHorizontal();
                    int oldVer = index == -1 ? 0 : settings.sceneVersions[index];
                    if (index == -1)
                    {
                        EditorGUILayout.LabelField(path.Replace(settings.resDir,""), "new", newSceneStyle);
                    }
                    else
                    {
                        int ver = EditorGUILayout.IntField(path.Replace(settings.resDir,""), oldVer);
                        if (oldVer != ver)
                        {
                            settings.sceneVersions[i] = ver;
                            serializedObject.ApplyModifiedProperties();
                            BMEditUtility.SaveSetting(settings);
                            Debug.Log("ApplyModifiedProperties scene version");
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                }
            }
            //重制所有场景版本号
            if (GUILayout.Button("Reset All Scenes Version"))
            {
                settings.sceneVersions = new List<int>();
                settings.scenePaths = new List<string>();
                serializedObject.ApplyModifiedProperties();
                BMEditUtility.SaveSetting(settings);
            }
        }

        private void GenerateAtlasSprite(bool forceGenerate)
        {
            if (GUILayout.Button("Generate Atlas Sprite"))
            {
                for (int i = 0; i < settings.atlasSpriteFolderList.Count; i++)
                {
                    string atlasDir = settings.atlasSpriteFolderList[i];
                    if(forceGenerate || CheckModify(atlasDir))
                        GenerateAtlasSpritePrefab(atlasDir);
                    else
                    {
                        Debug.Log(string.Format("There is not modify in atlas directory -- {0}", atlasDir));
                    }
                }
            }
        }
        private bool CheckModify(string atlasDir)
        {
            Dictionary<string, string> md5Map = BMEditUtility.GetDictionaryFromFile(Path.Combine(atlasDir, "manifest.txt"));
            bool modify = false;
            if (md5Map == null)
            {
                modify = true;
            }else
            {
                FileInfo[] resFiles = BMEditUtility.GetAllFiles(atlasDir, "*.*");
                for (int i = 0; i < resFiles.Length; i++)
                {
                    FileInfo info = resFiles[i];
                    if (info.FullName.EndsWith(".meta", StringComparison.Ordinal) || info.FullName.EndsWith(".txt", StringComparison.Ordinal))
                        continue;
                    string spriteFileName = Path.GetFileName(info.FullName);
                    string oldmd5;
                    string md5 = HashHelper.ComputeMD5(Path.Combine(atlasDir, spriteFileName));
                    if (md5Map.TryGetValue(spriteFileName, out oldmd5))
                    {
                        if (md5 != oldmd5)
                        {
                            modify = true;
                            break;
                        }
                    }
                    else
                    {
                        modify = true;
                        break;
                    }
                }
            }
            return modify;
        }
        private void GenerateAtlasSpritePrefab(string atlasDir)
        {
            Dictionary<string, string> md5Map = new Dictionary<string, string>();
            string dirName = Path.GetFileName(atlasDir);
            string outPath = settings.atlasSpritePrefabDir + dirName + "/";
            if (Directory.Exists(outPath))
                BMEditUtility.DelFolder(outPath);
            Directory.CreateDirectory(outPath);
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(atlasDir, "*.*");
            for (int j = 0; j < resFiles.Length; j++)
            {
                FileInfo info = resFiles[j];
                if (info.FullName.EndsWith(".meta", StringComparison.Ordinal) || info.FullName.EndsWith(".txt", StringComparison.Ordinal))
                    continue;
                string spriteFileName = Path.GetFileName(info.FullName);
                string md5 = HashHelper.ComputeMD5(Path.Combine(atlasDir, spriteFileName));
                md5Map.Add(spriteFileName, md5);
                string spriteName = Path.GetFileNameWithoutExtension(info.FullName);
                GameObject spritePrefab = new GameObject(spriteName);
                Image img = spritePrefab.AddComponent<Image>();
                string rPath = BMEditUtility.Absolute2Relativity(info.FullName);
                TextureImporter ti = AssetImporter.GetAtPath(rPath) as TextureImporter;
                if (ti)
                {
                    ti.spritePackingTag = "Atlas_" + dirName;
                    ti.SaveAndReimport();
                }
                img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(rPath);
                //spritePrefab.hideFlags = HideFlags.HideInHierarchy;
                PrefabUtility.SaveAsPrefabAsset(spritePrefab, outPath + spriteName + ".prefab");
                DestroyImmediate(spritePrefab);
            }

            //Save md5
            BMEditUtility.SaveDictionary(Path.Combine(atlasDir, "manifest.txt"), md5Map);
        }
        
        private void UpdatePackingTag(string dir)
        {
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(dir, "*.*");
            for (int i = 0; i < resFiles.Length; i++)
            {
                FileInfo info = resFiles[i];
                string rPath = BMEditUtility.Absolute2Relativity(info.FullName);
                if(Path.GetExtension(rPath) == ".meta")
                    continue;
                string dirName = Path.GetFileName(Path.GetDirectoryName(rPath));
                TextureImporter ti = AssetImporter.GetAtPath(rPath) as TextureImporter;
                if (ti)
                {
                    ti.spritePackingTag = "UISprite_" + dirName;
                    ti.SaveAndReimport();
                }
            }
        }
    }
}


