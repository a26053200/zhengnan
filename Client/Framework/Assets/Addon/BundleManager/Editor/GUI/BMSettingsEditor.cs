using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine.U2D;
using File = UnityEngine.Windows.File;
using Object = UnityEngine.Object;

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

           
        }

        private void GenerateAtlasSprite(bool forceGenerate)
        {
            for (int i = 0; i < _settings.atlasSpriteFolderList.Count; i++)
            {
                string atlasDir = _settings.atlasSpriteFolderList[i];
                if (forceGenerate || CheckModify(atlasDir))
                {
//                    AutoSetAtlasContents(atlasDir);
                    GenerateAtlasSpritePrefab(atlasDir);
                }
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
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(atlasDir, "*.*");
            for (int j = 0; j < resFiles.Length; j++)
            {
                FileInfo info = resFiles[j];
                if (info.FullName.EndsWith(".meta", StringComparison.Ordinal) || info.FullName.EndsWith(".txt", StringComparison.Ordinal))
                    continue;
                
                string dirName = Path.GetFileName(info.DirectoryName);
                string outPath = $"{_settings.atlasSpritePrefabDir}{dirName}/";
                string spriteFileName = Path.GetFileName(info.FullName);
                string spriteName = Path.GetFileNameWithoutExtension(info.FullName);
                string texPath = BMEditUtility.Absolute2Relativity(info.FullName);
                string md5 = HashHelper.ComputeMD5(texPath);
                md5Map.Add(texPath, md5);
                TextureImporter ti = AssetImporter.GetAtPath(texPath) as TextureImporter;
                if (ti)
                {
                    if (!Directory.Exists(outPath))
                        Directory.CreateDirectory(outPath);
                    GameObject spritePrefab = new GameObject(spriteName);
                    SpriteRenderer spriteRenderer = spritePrefab.AddComponent<SpriteRenderer>();
                    Vector4 border = ti.spriteBorder;
                    ti.spritePackingTag = "Atlas_" + dirName;
                    ti.SaveAndReimport();
                    spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texPath);;
                    //spritePrefab.hideFlags = HideFlags.HideInHierarchy;
                    var savePath = outPath + spriteName + ".prefab";
                    if(File.Exists(savePath))
                        File.Delete(savePath);
                    PrefabUtility.SaveAsPrefabAsset(spritePrefab, savePath);
                    //var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(rPath);
                    //var sprite = Sprite.Create(texture2D, new Rect(0,0,texture2D.width,texture2D.height), new Vector2(0.5f,0.5f), 400, 0,SpriteMeshType.FullRect,border);
                    DestroyImmediate(spritePrefab);
                }
            }

            //Save md5
            BMEditUtility.SaveDictionary(Path.Combine(atlasDir, "manifest.txt"), md5Map);
        }
        
        void AutoSetAtlasContents(string texturePath)
        {
            // 设置参数 可根据项目具体情况进行设置
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2,
            };
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                crunchedCompression = true,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            };
            DirectoryInfo root = new DirectoryInfo(texturePath);
            DirectoryInfo[] dirs = root.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                SpriteAtlas atlas = new SpriteAtlas();
                atlas.SetPackingSettings(packSetting);
                atlas.SetTextureSettings(textureSetting);
                atlas.SetPlatformSettings(platformSetting);
           
                // 这里我使用的是png图片，已经生成Sprite精灵了
                FileInfo[] files = dir.GetFiles("*.png");
                foreach (FileInfo file in files)
                {
                    string dirName = Path.GetFileName(file.DirectoryName);
                    string texPath = $"{texturePath}/{dirName}/{file.Name}";
                    atlas.Add(new[] {AssetDatabase.LoadAssetAtPath<Sprite>(texPath)});
                    //Debug.Log($"Create sprite {texPath}");
                }
                string atlasPath = $"{texturePath}/{dir.Name}.spriteatlas";
                //Debug.Log($"Create sprite {atlasPath}");
                AssetDatabase.CreateAsset(atlas, atlasPath);
                AssetDatabase.SaveAssets();
            }
            // 2、添加文件夹
            //Object obj = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Object));
            //atlas.Add(new[] {obj});
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


