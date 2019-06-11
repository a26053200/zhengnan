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

        private Shader shader;
        private int shaderLevel;

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            mat = obj as Material;
            shader = mat.shader;

            string[] forbid_shaders = Norm.GetIntance().Shader_Forbid.Split(',');
            shaderLevel = 0;
            for (int i = 0; i < forbid_shaders.Length; i++)
            {
                if (shader.name.ToLower().IndexOf(forbid_shaders[i].ToLower()) != -1)
                {
                    shaderLevel = 2;
                    errorNum += 1;
                    break;
                }
            }
        }

        public override void OnResourceGUI()
        {
            EditorGUILayout.BeginHorizontal();
            ResUtils.ColorLabelFieldTooltip("Shader", shader.name, string.Format("Shader Forbid: %d", Norm.GetIntance().Shader_Forbid), shaderLevel);
            EditorGUILayout.ObjectField("", mat.shader, typeof(Shader), false);
            EditorGUILayout.EndHorizontal();
        }
    }
}
