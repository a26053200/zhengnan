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

        BundleLoader bundleLoader;

        private void OnEnable()
        {
            bundleLoader = target as BundleLoader;
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("LoadText"))
            {
                bundleLoader.LoadBundleData();
            }
            if (GUILayout.Button("LoadBundle"))
            {
                AssetBundle ab = bundleLoader.LoadAssetBundle("Assets/Lua/Main.lua.a19cffbd6db217c8a9f41869010f8a9e.txt");
                TextAsset ta = ab.LoadAsset<TextAsset>("Assets/Lua/Main.lua.a19cffbd6db217c8a9f41869010f8a9e.txt");
                Debug.Log(ta.text);
            }
        }
    }
}


