using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

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

        private void OnEnable()
        {
            setting = target as BMSettings;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if(GUILayout.Button("Generate Atlas Sprite Prefab"))
            {
                for (int i = 0; i < setting.atlasSpriteFolderList.Count; i++)
                {
                    string atlasDir = setting.atlasSpriteFolderList[i];
                    string dirName = Path.GetFileName(atlasDir);
                    string outPath = setting.atlasSpritePrefabDir + dirName + "/";
                    if (Directory.Exists(outPath))
                        BMEditUtility.DelFolder(outPath);
                    Directory.CreateDirectory(outPath);
                    FileInfo[] resFiles = BMEditUtility.GetAllFiles(atlasDir, "*.*");
                    for (int j = 0; j < resFiles.Length; j++)
                    {
                        FileInfo info = resFiles[j];
                        if (info.FullName.EndsWith(".meta"))
                            continue;
                        string spriteName = Path.GetFileNameWithoutExtension(info.FullName);
                        GameObject spritePrefab = new GameObject(spriteName);
                        Image img = spritePrefab.AddComponent<Image>();
                        string rPath = BMEditUtility.Absolute2Relativity(info.FullName);
                        img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(rPath);
                        //spritePrefab.hideFlags = HideFlags.HideInHierarchy;
                        PrefabUtility.SaveAsPrefabAsset(spritePrefab, outPath + spriteName + ".prefab");
                    }
                }
            }
        }
    }
}


