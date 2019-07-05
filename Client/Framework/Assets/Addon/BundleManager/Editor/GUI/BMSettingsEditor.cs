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
        BMSettings setting;
        bool forceGenerate;

        private void OnEnable()
        {
            setting = target as BMSettings;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            forceGenerate = EditorGUILayout.ToggleLeft("Force Generate", forceGenerate);
            if (GUILayout.Button("Generate Atlas Sprite"))
            {
                for (int i = 0; i < setting.atlasSpriteFolderList.Count; i++)
                {
                    string atlasDir = setting.atlasSpriteFolderList[i];
                    if(forceGenerate || CheckModify(atlasDir))
                        GenerateAtlasSpritePrefab(atlasDir);
                    else
                    {
                        Debug.Log(string.Format("There is not modify in atlas directory -- {0}", atlasDir));
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
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
                    if (info.FullName.EndsWith(".meta") || info.FullName.EndsWith(".txt"))
                        continue;
                    string spriteFileName = Path.GetFileName(info.FullName);
                    string oldmd5;
                    string md5 = BMEditUtility.GetFileMD5(Path.Combine(atlasDir, spriteFileName));
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
            string outPath = setting.atlasSpritePrefabDir + dirName + "/";
            if (Directory.Exists(outPath))
                BMEditUtility.DelFolder(outPath);
            Directory.CreateDirectory(outPath);
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(atlasDir, "*.*");
            for (int j = 0; j < resFiles.Length; j++)
            {
                FileInfo info = resFiles[j];
                if (info.FullName.EndsWith(".meta") || info.FullName.EndsWith(".txt"))
                    continue;
                string spriteFileName = Path.GetFileName(info.FullName);
                string md5 = BMEditUtility.GetFileMD5(Path.Combine(atlasDir, spriteFileName));
                md5Map.Add(spriteFileName, md5);
                string spriteName = Path.GetFileNameWithoutExtension(info.FullName);
                GameObject spritePrefab = new GameObject(spriteName);
                Image img = spritePrefab.AddComponent<Image>();
                string rPath = BMEditUtility.Absolute2Relativity(info.FullName);
                img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(rPath);
                //spritePrefab.hideFlags = HideFlags.HideInHierarchy;
                PrefabUtility.SaveAsPrefabAsset(spritePrefab, outPath + spriteName + ".prefab");
                DestroyImmediate(spritePrefab);
            }

            //Save md5
            BMEditUtility.SaveDictionary(Path.Combine(atlasDir, "manifest.txt"), md5Map);
        }
    }
}


