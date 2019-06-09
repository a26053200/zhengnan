using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ResourceAuditing
{
    public class MaterialDetails : ResourceDetail
    {
        public string path;
        public int hashCode;

        public MaterialDetails(string path, string name) : base(path, name)
        {
            resources = new List<Resource>();
        }
    }

    public class MaterialResource : Resource
    {
        private Material mat;

        public Material Material
        {
            get { return mat; }
        }

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            mat = obj as Material;
        }

        public override void OnResourceGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", name);
            EditorGUILayout.ObjectField("Shader", mat.shader, typeof(Shader), false);
            EditorGUILayout.EndHorizontal();
        }
    }
}
