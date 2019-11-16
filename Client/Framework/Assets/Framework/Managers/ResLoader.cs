using System;
using UnityEngine;
using System.Collections.Generic;
using BM;
using UnityEngine.Events;
using System.Collections;
using System.IO;

namespace Framework
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/20 22:43:36</para>
    /// </summary> 
    public class ResLoader : BaseManager
    {
        class ResLoaderInfo
        {
            public string assetPath;
            public UnityAction<AssetBundle> OnAssetBundleLoaded;
        }

        BundleLoader bundleLoader;
        Queue<ResLoaderInfo> loadQueue;
        ResLoaderInfo currLoadInfo;
        public override void Initialize()
        {
            if (GlobalConsts.isRunningInMobileDevice || GlobalConsts.isResBundleMode)
            {
                GameObject obj = new GameObject("[BM]");
                DontDestroyOnLoad(obj);
                bundleLoader = obj.AddComponent<BundleLoader>();
                bundleLoader.LoadBundleData();
                loadQueue = new Queue<ResLoaderInfo>();
            }
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

        public IEnumerator AddLoadAssetBundleAsync(string assetPath, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            ResLoaderInfo info = new ResLoaderInfo()
            {
                assetPath = assetPath,
                OnAssetBundleLoaded = OnAssetBundleLoaded,
            };
            loadQueue.Enqueue(info);
            if (currLoadInfo == null)
            {
                yield return StartCoroutine(LoadNext());
            }
            
        }

        public IEnumerator LoadNext()
        {
            if (loadQueue.Count > 0)
            {
                currLoadInfo = loadQueue.Dequeue();
                if (BundleLoadState.Loading == bundleLoader.GetBundleStateByAssetPath(currLoadInfo.assetPath))
                {
                    Logger.Log("Bundle: {0} is Loading", currLoadInfo.assetPath);
                    loadQueue.Enqueue(currLoadInfo);//重新排队列
                }
                else
                {
                    yield return bundleLoader.LoadAssetBundleAsync(currLoadInfo.assetPath, currLoadInfo.OnAssetBundleLoaded);
                    
                }
                yield return StartCoroutine(LoadNext());
            }
            else
            {
                currLoadInfo = null;
            }
        }
            

        public IEnumerator LoadAssetBundleAsync(string assetPath, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            ResLoaderInfo info = new ResLoaderInfo()
            {
                assetPath = assetPath,
                OnAssetBundleLoaded = OnAssetBundleLoaded,
            };
            yield return bundleLoader.LoadAssetBundleAsync(assetPath, OnAssetBundleLoaded);
            if (BundleLoadState.Loading == bundleLoader.GetBundleStateByAssetPath(assetPath))
                Logger.Log("Bundle: {0} is Loading", assetPath);
        }
    }
}
    

