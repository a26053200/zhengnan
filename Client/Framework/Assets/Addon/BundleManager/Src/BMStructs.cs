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
        public List<string> assetPaths;
        public List<string> dependencePaths;
    }

    public class BundleLoadInfo
    {
        public string bundleName;
        public UnityAction<AssetBundle> OnAssetBundleLoaded;
    }
}
    

