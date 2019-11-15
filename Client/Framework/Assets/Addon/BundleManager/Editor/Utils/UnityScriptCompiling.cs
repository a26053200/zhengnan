using UnityEditor;

namespace BM
{
    [InitializeOnLoad]
    public class UnityScriptCompiling : AssetPostprocessor
    {
        static UnityScriptCompiling()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (!EditorApplication.isCompiling)
            {
                if (EditorApplication.update != null)
                    EditorApplication.update -= Update;
                EditorUtility.ClearProgressBar();
            }
        }
    }
}