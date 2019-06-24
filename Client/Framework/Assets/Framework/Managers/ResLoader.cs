using System;
using UnityEngine;
using System.Collections.Generic;
using BM;
using UnityEngine.Events;
using System.Collections;

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


        public AssetBundle GetBundleByAssetPath(string assetPath)
        {
            if (BundleLoadState.Loading == bundleLoader.GetBundleStateByAssetPath(assetPath))
            {
                Logger.LogError("Bundle: {0} is Loading, You can't load bundle when the bundle is loading by other task!!!", assetPath);
                return null;
            }
            else
                return bundleLoader.LoadAssetBundle(assetPath);
        }

        public AssetBundle GetBundleByBundleName(string bundleName)
        {
            return bundleLoader.LoadBundleSync(bundleName);
        }

        public IEnumerator LoadAssetBundleAsync(string assetPath, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            if (BundleLoadState.Loading == bundleLoader.GetBundleStateByAssetPath(assetPath))
                Logger.Info("Bundle: {0} is Loading", assetPath);
            else
                yield return bundleLoader.LoadAssetBundleAsync(assetPath, OnAssetBundleLoaded); 
        }
    }
}
    

