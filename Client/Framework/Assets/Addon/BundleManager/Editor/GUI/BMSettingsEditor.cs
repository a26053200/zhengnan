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
        private BMSettings _settings;
        private GUIStyle _newSceneStyle;
        private Dictionary<string, string> _scenesVersionDict;

        private void OnEnable()
        {
            _settings = target as BMSettings;
            _newSceneStyle = new GUIStyle();
            _newSceneStyle.normal.textColor = Color.green;
            _scenesVersionDict = BMEditUtility.GetDictionaryFromFile(_settings.scenesVersionFile);
            if (_scenesVersionDict == null)
                _scenesVersionDict = new Dictionary<string, string>();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            

            if (!string.IsNullOrEmpty(_settings.scenesVersionFile))
            {
                EditorGUILayout.Space();
                foreach (var folder in _settings.scenesFolderList)
                {
                    FileInfo[] sceneFiles = BMEditUtility.GetAllFiles(folder, _settings.scenesPattern);
                    for (int i = 0; i < sceneFiles.Length; i++)
                    {
                        string path = BMEditUtility.Absolute2Relativity(sceneFiles[i].DirectoryName) + "/" + sceneFiles[i].Name; //相对路径
                        if (_scenesVersionDict.TryGetValue(path, out string version))
                        {
                            var oldVer = version;
                            version = EditorGUILayout.TextField(path.Replace(_settings.resDir,""), version);
                            if (version != oldVer)
                            {
                                _scenesVersionDict[path] = version;
                                BMEditUtility.SaveDictionary(_settings.scenesVersionFile, _scenesVersionDict);
                            }
                        }
                        else
                            EditorGUILayout.TextField(path.Replace(_settings.resDir,""), "0");
                    }
                }
            
                if (GUILayout.Button("Save Scene Version"))
                {
                    BMEditUtility.SaveDictionary(_settings.scenesVersionFile,_scenesVersionDict);
                }
                //重制所有场景版本号
                if (GUILayout.Button("Reset All Scenes Version"))
                {
                    _scenesVersionDict = new Dictionary<string, string>();
                    BMEditUtility.SaveDictionary(_settings.scenesVersionFile, _scenesVersionDict);
//                _settings.sceneVersions = new List<int>();
//                _settings.scenePaths = new List<string>();
//                serializedObject.ApplyModifiedProperties();
//                BMEditUtility.SaveSetting(_settings);
                }
            }
            
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
            _settings.uiSpriteDir = EditorGUILayout.TextField("UI Sprite", _settings.uiSpriteDir);
            if (!string.IsNullOrEmpty(_settings.uiSpriteDir))
            {
                if (GUILayout.Button("Update All Packing Tag"))
                {
                    UpdatePackingTag(_settings.uiSpriteDir);
                }
            }
            
//            
//            EditorGUILayout.Space();
//            foreach (var folder in _settings.scenesFolderList)
//            {
//                FileInfo[] sceneFiles = BMEditUtility.GetAllFiles(folder, _settings.scenesPattern);
//                    
//                for (int i = 0; i < sceneFiles.Length; i++)
//                {
//                    string path = BMEditUtility.Absolute2Relativity(sceneFiles[i].DirectoryName) + "/" + sceneFiles[i].Name; //相对路径
//                    int index = _settings.scenePaths.IndexOf(path);
//                    EditorGUILayout.BeginHorizontal();
//                    int oldVer = index == -1 ? 0 : _settings.sceneVersions[index];
//                    if (index == -1)
//                    {
//                        EditorGUILayout.LabelField(path.Replace(_settings.resDir,""), "new", _newSceneStyle);
//                    }
//                    else
//                    {
//                        int ver = EditorGUILayout.IntField(path.Replace(_settings.resDir,""), oldVer);
//                        if (oldVer != ver)
//                        {
//                            _settings.sceneVersions[i] = ver;
//                            serializedObject.ApplyModifiedProperties();
//                            BMEditUtility.SaveSetting(_settings);
//                            Debug.Log("ApplyModifiedProperties scene version");
//                        }
//                    }
//                    
//                    EditorGUILayout.EndHorizontal();
//                    
//                }
//            }
           
        }

        private void GenerateAtlasSprite(bool forceGenerate)
        {
            for (int i = 0; i < _settings.atlasSpriteFolderList.Count; i++)
            {
                string atlasDir = _settings.atlasSpriteFolderList[i];
                if(forceGenerate || CheckModify(atlasDir))
                    GenerateAtlasSpritePrefab(atlasDir);
                else
                {
                    Debug.Log($"There is not modify in atlas directory -- {atlasDir}");
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
            string outPath = _settings.atlasSpritePrefabDir + dirName + "/";
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


