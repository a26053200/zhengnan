using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;

namespace ResourceAuditing
{
    public class TextureDetails : IEquatable<TextureDetails>
    {
        public bool isOpen = false;
        public bool isClick = false;

        public bool isCubeMap;
        public string name;
        public string path;
        public int memSizeKB;
        public Texture texture;
        public FileInfo fileInfo;
        public TextureFormat format;
        public int mipMapCount;
        public List<Object> FoundInMaterials = new List<Object>();
        public List<Object> FoundInRenderers = new List<Object>();
        public List<Object> FoundInAnimators = new List<Object>();
        public List<Object> FoundInScripts = new List<Object>();
        public List<Object> FoundInGraphics = new List<Object>();
        public List<Object> FoundInButtons = new List<Object>();
        public bool isSky;
        public bool instance;
        public bool isgui;
        public TextureDetails()
        {

        }

        public bool Equals(TextureDetails other)
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
