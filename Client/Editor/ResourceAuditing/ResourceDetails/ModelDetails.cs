using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ResourceAuditing
{
    public class ModelDetails : ResourceDetail
    {
        public string path;
        public int hashCode;

        public ModelDetails(string path, string name) : base(path, name)
        {
            resources = new List<Resource>();
        }

    }
    public class ModelResource : Resource
    {
        private Object modelOrgObj;
        public ModelImporter modelImporter;

        public Object ModelOrgObj
        {
            get { return modelOrgObj; }
        }

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            modelOrgObj = obj;
            modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
        }

        public override void OnResourceGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GameObject modelGameObject = modelOrgObj as GameObject;
            ResUtils.ColorLabelField("Read&Write", modelImporter.isReadable.ToString(), !modelImporter.isReadable);
            ResUtils.ColorLabelField("Optimize Mesh", modelImporter.optimizeMesh.ToString(), modelImporter.optimizeMesh ? 1 : 0);
            EditorGUILayout.EndHorizontal();

            Renderer[] renderers = modelGameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer render = renderers[i];
                if (render is MeshRenderer)
                {
                    MeshFilter mf = render.GetComponent<MeshFilter>();
                    DrawMeshInfo(mf.sharedMesh);
                }
                else if (render is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer smr = render as SkinnedMeshRenderer;
                    DrawMeshInfo(smr.sharedMesh);
                }

            }

            ModelImporterClipAnimation[] mClips = modelImporter.clipAnimations;
            for (int i = 0; i < mClips.Length; i++)
            {

            }
        }

        private void DrawMeshInfo(Mesh mesh)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("", mesh, typeof(Mesh), false);
            int level = 0;
            if (mesh.triangles.Length > 1500 && mesh.triangles.Length <= 3000)
                level = 1;
            else if (mesh.triangles.Length > 3000)
                level = 2;
            ResUtils.ColorLabelFieldTooltip("tris", mesh.triangles.Length.ToString(), "Max tris num is 3000", level, 150);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAnimationClipInfo(ModelImporterClipAnimation clip)
        {

        }
    }
}
    