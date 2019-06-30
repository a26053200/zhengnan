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

            string[] forbid_shaders = ResourceAuditingSetting.GetIntance().Shader_Forbid.Split(',');
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
        const string Title_Empty = "";

        const string Title_Shader = "Shader Name";
        const string Format_Shader = "Shader Forbid: %d";
        public override void OnResourceGUI()
        {
            EditorGUILayout.BeginHorizontal();
            ResUtils.ColorLabelFieldTooltip(Title_Shader, shader.name, string.Format(Format_Shader, ResourceAuditingSetting.GetIntance().Shader_Forbid), shaderLevel);
            EditorGUILayout.ObjectField(Title_Empty, mat.shader, typeof(Shader), false);
            EditorGUILayout.EndHorizontal();
        }
    }
}
