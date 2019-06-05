using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;

namespace ResourceAuditing
{
    public class TextureDetails
    {
        public bool isOpen = false;
        public bool isClick = false;

        public string md5;
        public int hashCode;
        public List<TextureResource> resources;


        public TextureDetails()
        {
            resources = new List<TextureResource>();
        }

        
    }
    public class TextureResource : IEquatable<TextureResource>
    {
        public string name;
        public string path;
        public Texture texture;
        public FileInfo fileInfo;
        public TextureFormat format;

        public bool Equals(TextureResource other)
        {
            return texture != null && other.texture != null &&
                texture.GetNativeTexturePtr() == other.texture.GetNativeTexturePtr();
        }

        public override int GetHashCode()
        {
            return (int)texture.GetNativeTexturePtr();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TextureDetails);
        }
    }
}
