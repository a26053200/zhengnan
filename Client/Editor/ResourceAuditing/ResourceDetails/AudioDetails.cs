using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace ResourceAuditing
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/7/1 22:16:37</para>
    /// </summary> 
    public class AudioDetails : ResourceDetail
    {
        public string path;
        public int hashCode;

        public AudioDetails(string path, string name) : base(path, name)
        {
            resources = new List<Resource>();
        }
    }

    public class AudioResource : Resource
    {
        private AudioClip clip;
        private AudioImporter audioImporter;

        public override void SetResObj(Object obj)
        {
            clip = obj as AudioClip;
            audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
        }
        public override void OnResourceGUI()
        {
            
        }
    }
}


