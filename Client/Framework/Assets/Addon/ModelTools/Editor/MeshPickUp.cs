using System.IO;
using UnityEditor;
using UnityEngine;

namespace Addon.ModelTools.Editor
{
    public static class MeshPickUp
    {
        [MenuItem("Assets/Pick up mesh")]
        static void PickUpMesh()
        {
            Object[] selObjs = Selection.objects;
            for (int i = 0; selObjs != null && i < selObjs.Length; i++)
            {
                Object fileObj = selObjs[i];
                GameObject go = fileObj as GameObject;
                if (!go) continue;
                MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);
                if (meshFilters.Length > 0)
                {
                    string savePath = AssetDatabase.GetAssetPath(go);
                    string meshName = Path.GetFileNameWithoutExtension(savePath);
                    savePath = savePath.Replace(Path.GetFileName(savePath), "");
                    for (int j = 0; j < meshFilters.Length; j++)
                    {
                        string name = meshFilters[j].sharedMesh.name;
                        Mesh mesh = Object.Instantiate(meshFilters[j].sharedMesh);
                        string path = savePath + name + ".mesh";
                        AssetDatabase.CreateAsset(mesh, path);
                        AssetDatabase.SaveAssets();
                    }

                    AssetDatabase.Refresh();
                }
            }
        }
    }
}