using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BM
{
    public class BundleInfo
    {
        public string bundleName;
        public string buildMd5;
        public string fileHash;
        public int crc;
        public BuildType buildType;
        public List<string> assetPaths;
        public List<string> dependencePaths;
        public List<BundleInfo> dependenceChildren;
        public BundleInfo parent;
        public BundleReferenceInfo bundleReference;

#if UNITY_EDITOR
        public bool showChildren;
#endif
    }

    public class BundleReferenceInfo
    {
        public string bundleName;
        public BuildType buildType;
        public BundleLoadState state;
        public int count;//引用计数
        public AssetBundle assetBundle;
    }
}
    

