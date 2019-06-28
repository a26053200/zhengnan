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
        private ModelImporter modelImporter;

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            modelOrgObj = obj;
            modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            GameObject modelGameObject = modelOrgObj as GameObject;

            if (modelImporter.isReadable)
                errorNum++;
            if (!modelImporter.optimizeMesh)
                warnNum++;

            Renderer[] renderers = modelGameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer render = renderers[i];
                if (render is MeshRenderer)
                {
                    MeshFilter mf = render.GetComponent<MeshFilter>();
                    MeshInfo(mf.sharedMesh);
                }
                else if (render is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer smr = render as SkinnedMeshRenderer;
                    MeshInfo(smr.sharedMesh);
                }
            }
        }
        const string Title_Platform = "Platform";

        const string Title_ReadAndWrite = "Read & Write";
        const string Title_OptimizeMesh = "Optimize Mesh";

        const string Title_TrisNum = "Tris num";
        const string Format_TrisNum = "Max tris num is {0}";

        public override void OnResourceGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GameObject modelGameObject = modelOrgObj as GameObject;
            ResUtils.ColorLabelField(Title_ReadAndWrite, modelImporter.isReadable.ToString(), !modelImporter.isReadable);
            ResUtils.ColorLabelField(Title_OptimizeMesh, modelImporter.optimizeMesh.ToString(), modelImporter.optimizeMesh ? 1 : 0);
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
            //ModelImporterClipAnimation[] mClips = modelImporter.clipAnimations;
            //for (int i = 0; i < mClips.Length; i++)
            //{

            //}
        }

        private void MeshInfo(Mesh mesh)
        {
            if (mesh.triangles.Length > Norm.GetIntance().Mesh_Recommend_TrisNum && mesh.triangles.Length <= Norm.GetIntance().Mesh_Max_TrisNum)
                warnNum++;
            else if (mesh.triangles.Length > Norm.GetIntance().Mesh_Max_TrisNum)
                errorNum++;
        }
        private void DrawMeshInfo(Mesh mesh)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("", mesh, typeof(Mesh), false);
            int level = 0;
            if (mesh.triangles.Length > Norm.GetIntance().Mesh_Recommend_TrisNum && mesh.triangles.Length <= Norm.GetIntance().Mesh_Max_TrisNum)
                level = 1;
            else if (mesh.triangles.Length > Norm.GetIntance().Mesh_Max_TrisNum)
                level = 2;
            ResUtils.ColorLabelFieldTooltip(Title_TrisNum, mesh.triangles.Length.ToString(), string.Format(Format_TrisNum, Norm.GetIntance().Mesh_Max_TrisNum), level);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAnimationClipInfo(ModelImporterClipAnimation clip)
        {

        }
    }
}
    