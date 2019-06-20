using System;
using UnityEngine;
using System.Collections.Generic;
using BM;

namespace Framework
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/20 22:43:36</para>
    /// </summary> 
    public class ResLoader : BaseManager
    {
        BundleLoader bundleLoader;

        public override void Initialize()
        {
            bundleLoader = gameObject.AddComponent<BundleLoader>();
            bundleLoader.LoadBundleData();
        }

        public AssetBundle GetBundle(string bundleName, bool path2name = false)
        {
            return bundleLoader.LoadAssetBundle(bundleName, path2name);
        }
    }
}
    

