using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;
using UnityEditor;

namespace ResourceAuditing
{
    public class TextureDetails : ResourceDetail
    {
        public string path;
        public int hashCode;

        public TextureDetails(string path, string name) :base(path, name)
        {
            resources = new List<Resource>();
        }
        
    }

    public class TextureResource : Resource
    {
        private Texture2D texture;
        public TextureFormat format;

        public Texture2D Texture
        {
            get { return texture; }
        }

        public override void SetResObj(Object obj)
        {
            resObj = obj;
            texture = obj as Texture2D;
        }

        public override void OnResourceGUI()
        {
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("", name);
            EditorGUILayout.LabelField("Format", texture.format.ToString() + " - " + (int)texture.format);
            //EditorGUILayout.EnumFlagsField("", texture.format);
            EditorGUILayout.EndHorizontal();
            
            
        }
    }
}
