using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/19 23:16:53</para>
    /// </summary> 
    [CustomEditor(typeof(BundleLoader))]
    public class BundleLoaderEditor : Editor
    {
        private const string Title_DependenceChildren = "Dependence Children";

        private BundleLoader _bundleLoader;
        private bool _showNotLoaded;
        private readonly Dictionary<string, bool> showFolderDict = new Dictionary<string, bool>();
        private readonly Dictionary<string, List<BundleInfo>> bundleInfoListDict = new Dictionary<string, List<BundleInfo>>();
        
        private void OnEnable()
        {
            _bundleLoader = target as BundleLoader;
            if (_bundleLoader != null)
                foreach (var bundleInfo in _bundleLoader.bundleInfos)
                {
                    string path = BMUtility.Name2Path(bundleInfo.bundleName);
                    int index = path.IndexOf("/", StringComparison.Ordinal);
                    string folderName;
                    if (index > 0)
                        folderName = path.Substring(0, index);
                    else
                        folderName = "other";
                    if (!showFolderDict.ContainsKey(folderName))
                    {
                        showFolderDict.Add(folderName, false);
                        bundleInfoListDict.Add(folderName, new List<BundleInfo>());
                    }
                    bundleInfoListDict.TryGetValue(folderName, out List<BundleInfo> list);
                    list.Add(bundleInfo);
                }
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            _showNotLoaded = EditorGUILayout.ToggleLeft("Show Not Loaded", _showNotLoaded);
            foreach (var folderName in bundleInfoListDict.Keys)
            {
                showFolderDict[folderName] = EditorGUILayout.Foldout(showFolderDict[folderName], folderName);
                if (showFolderDict[folderName])
                {
                    foreach (var bundleInfo in bundleInfoListDict[folderName])
                    {
                        if(_showNotLoaded || bundleInfo.bundleReference != null)
                            displayBundleInfo(bundleInfo, false);
                    }
                }
            }
        }

        void displayBundleInfo(BundleInfo bundleInfo, bool isChild)
        {
            EditorGUILayout.BeginHorizontal();
            bundleInfo.showContent = EditorGUILayout.ToggleLeft(new GUIContent("", "show content"), bundleInfo.showContent, GUILayout.Width(16));
            EditorGUILayout.TextField("count:" + bundleInfo.bundleReference?.count, BMUtility.Name2Path(bundleInfo.bundleName));
            EditorGUILayout.EndHorizontal();
            if (bundleInfo.showContent && bundleInfo.bundleReference != null && bundleInfo.bundleReference.assetBundle)
            {
                Object[] abList = bundleInfo.bundleReference.assetBundle.LoadAllAssets();
                for (int i = 0; i < abList.Length; i++)
                {
                    EditorGUILayout.ObjectField("", abList[i], typeof(Object),false);
                }
            }
            if (!isChild && bundleInfo.dependenceChildren != null)
            {
                EditorGUI.indentLevel++;
                bundleInfo.showChildren = EditorGUILayout.Foldout(bundleInfo.showChildren, Title_DependenceChildren);
                if (bundleInfo.showChildren)
                {
                    foreach (var item in bundleInfo.dependenceChildren)
                    {
                        EditorGUILayout.LabelField(BMUtility.Name2Path(item.bundleName));
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < item.assetPaths.Count; i++)
                            EditorGUILayout.TextField(item.assetPaths[i]);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }

}


