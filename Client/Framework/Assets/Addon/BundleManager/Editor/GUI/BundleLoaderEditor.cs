using UnityEngine;
using UnityEditor;

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
        const string Title_DependenceChildren = "Dependence Children";

        BundleLoader bundleLoader;
        bool showNotLoaded;
        private void OnEnable()
        {
            bundleLoader = target as BundleLoader;
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            showNotLoaded = EditorGUILayout.ToggleLeft("Show Not Loaded", showNotLoaded);
            foreach (var bundleInfo in bundleLoader.bundleInfos)
            {
                if(showNotLoaded || bundleInfo.bundleReference != null)
                {
                    displayBundleInfo(bundleInfo, false);
                }
            }
        }

        void displayBundleInfo(BundleInfo bundleInfo, bool isChild)
        {
            //BMEditUtility.DrawHorizontalSplitter();
            //EditorGUI.BeginDisabledGroup(true);
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(BMUtility.Name2Path(bundleInfo.bundleName));
            GUILayout.Label("[" + bundleInfo.buildType.ToString() + "]");
            if (bundleInfo.bundleReference != null)
            {
                GUILayout.Label("count:" + bundleInfo.bundleReference.count);
            }
            EditorGUILayout.EndHorizontal();
            if (bundleInfo.bundleReference != null)
            {
                //EditorGUILayout.ObjectField("", bundleInfo.bundleReference.assetBundle, typeof(AssetBundle), false);
            }
            //EditorGUI.EndDisabledGroup();
            if (!isChild && bundleInfo.dependenceChildren != null)
            {
                EditorGUI.indentLevel++;
                bundleInfo.showChildren = EditorGUILayout.Foldout(bundleInfo.showChildren, Title_DependenceChildren);
                if (bundleInfo.showChildren)
                {
                    foreach (var item in bundleInfo.dependenceChildren)
                    {
                        displayBundleInfo(item, true);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }

}


