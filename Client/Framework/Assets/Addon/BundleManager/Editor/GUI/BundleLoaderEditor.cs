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
            
        }
    }
}


