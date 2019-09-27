using System;
using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System.Collections;
using UnityEngine.Events;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/19 1:09:53</para>
    /// </summary> 
    public class BundleLoader : MonoBehaviour
    {
        private JsonData bundleJsonData;

        public List<BundleInfo> bundleInfos { get; private set; }
        public Dictionary<string, BundleInfo> bundleInfoDict { get; private set; }

        private void Awake()
        {
            
        }

        public void LoadBundleData()
        {
            string path = getFilePath(BMConfig.BundlDataFile);
            Debug.LogFormat("Load bundle data:{0}", path);
            string bundleData = BMUtility.LoadText(path);
            JsonData jsonData = JsonMapper.ToObject(bundleData);
            //Debug.Log(bundleData);

            bundleInfos = new List<BundleInfo>();
            bundleInfoDict = new Dictionary<string, BundleInfo>();

            JsonData bundleInfoJson = jsonData["bundles"];
            for (int i = 0; i < bundleInfoJson.Count; i++)
            {
                BundleInfo bundleInfo = new BundleInfo()
                {
                    bundleName = bundleInfoJson[i]["bundleName"].ToString(),
                    buildMd5 = bundleInfoJson[i]["buildMd5"].ToString(),
                    buildType = (BuildType)Enum.Parse(typeof(BuildType), bundleInfoJson[i]["buildType"].ToString()),
                    assetPaths = BMUtility.JsonToArray(bundleInfoJson[i], "assetPaths"),
                    dependencePaths = BMUtility.JsonToArray(bundleInfoJson[i], "dependencePaths"),
                };
                bundleInfos.Add(bundleInfo);
                
                for (int j = 0; j < bundleInfo.assetPaths.Count; j++)
                {
                    bundleInfoDict.Add(bundleInfo.assetPaths[j], bundleInfo);
                }
            }
            
        }

        //=======================
        // 同步加载
        //=======================

        public AssetBundle LoadAssetBundle(string assetPath)
        {
            BundleInfo bundleInfo = GetBundleInfo(assetPath);
            //如果该资源有依赖,则优先加载依赖
            for (int i = 0; i < bundleInfo.dependencePaths.Count; i++)
            {
                LoadAssetBundle(bundleInfo.dependencePaths[i]);
            }
            return LoadBundle(bundleInfo);
        }

        public AssetBundle LoadBundleSync(string bundleName)
        {
            BundleInfo bundleInfo = null;
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                if (bundleInfos[i].bundleName == bundleName)
                {
                    bundleInfo = bundleInfos[i];
                }
            }
            if (bundleInfo != null)
                return LoadBundle(bundleInfo);
            else
                return null;
        }

        private AssetBundle LoadBundle(BundleInfo bundleInfo)
        {
            AssetBundle assetBundle = null;
            string path = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
            if (bundleInfo.bundleReference == null)
            {
                string bundePath = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
                Debug.LogFormat("Load a new bundle:" + bundePath);
                assetBundle = AssetBundle.LoadFromFile(bundePath);
                if (assetBundle == null)
                {
                    Debug.LogErrorFormat("The AssetBundle '{0}' load fail!", path);
                    return null;
                }
                else
                {
                    BundleReferenceInfo bundleReferenceInfo = new BundleReferenceInfo()
                    {
                        assetBundle = assetBundle,
                        count = 1,
                        buildType = bundleInfo.buildType,
                        state = BundleLoadState.LoadComplete,
                    };
                    bundleInfo.bundleReference = bundleReferenceInfo;
                }
            }
            else
            {
                assetBundle = bundleInfo.bundleReference.assetBundle;
                bundleInfo.bundleReference.count += 1;
            }
            Debug.LogFormat("The AssetBundle '{0}' load success!", path);
            return assetBundle;
        }

        //=======================
        // 异步加载
        //=======================

        public IEnumerator LoadAssetBundleAsync(string assetPath, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            BundleInfo bundleInfo = GetBundleInfo(assetPath);
            if (bundleInfo != null)
            {
                //如果该资源有依赖,则优先加载依赖
                for (int i = 0; i < bundleInfo.dependencePaths.Count; i++)
                {
                    BundleInfo chindInfo = GetBundleInfo(assetPath);
                    chindInfo.parent = bundleInfo;
                    if (bundleInfo.dependenceChildren == null)
                        bundleInfo.dependenceChildren = new List<BundleInfo>();
                    bundleInfo.dependenceChildren.Add(chindInfo);
                    yield return LoadAssetBundleAsync(bundleInfo.dependencePaths[i], null);
                }
                yield return LoadBundleAsync(bundleInfo, OnAssetBundleLoaded);
            }
        }


        public IEnumerator LoadBundleAsync(BundleInfo bundleInfo, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            string path = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
            if (bundleInfo.bundleReference != null)
            {//就算 缓存池里面有 也要模拟异步加载
                yield return new WaitForEndOfFrame();
                if (OnAssetBundleLoaded != null)
                {
                    OnAssetBundleLoaded(bundleInfo.bundleReference.assetBundle);
                    bundleInfo.bundleReference.count += 1;
                }
            }
            else
            {
                BundleReferenceInfo bundleReferenceInfo = new BundleReferenceInfo()
                {
                    state = BundleLoadState.Loading,
                    count = 1,
                    buildType = bundleInfo.buildType,
                };
                AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
                yield return assetBundleCreateRequest;
                if (assetBundleCreateRequest.assetBundle == null)
                {
                    Debug.LogErrorFormat("Failed to load AssetBundle '{0}' !", path);
                }
                bundleReferenceInfo.assetBundle = assetBundleCreateRequest.assetBundle;
                bundleReferenceInfo.state = BundleLoadState.LoadComplete;
                bundleInfo.bundleReference = bundleReferenceInfo;
                if (OnAssetBundleLoaded != null)
                    OnAssetBundleLoaded(assetBundleCreateRequest.assetBundle);
            }
        }

        //获取BundleInfo
        private BundleInfo GetBundleInfo(string assetPath)
        {
            BundleInfo bundleInfo;
            if (bundleInfoDict.TryGetValue(assetPath, out bundleInfo))
                return bundleInfo;
            else
                Debug.LogErrorFormat("No path is '{0}' bundle.", assetPath);
            return null;
        }
        //=======================
        // 工具函数
        //=======================

        public BundleLoadState GetBundleStateByAssetPath(string assetPath)
        {
            BundleInfo bundleInfo = GetBundleInfo(assetPath);
            if (bundleInfo.bundleReference != null)
                return bundleInfo.bundleReference.state;
            else
                return BundleLoadState.None;
        }
        string getFilePath(string fileName)
        {
            string path = Path.Combine(BMConfig.RawDir, fileName);
            if (!BMUtility.FileExists(path))
                path = Path.Combine(BMConfig.ReadonlyDir, fileName);
            return path;
        }


        //=======================
        // 行为函数
        //=======================

        void OnDestroy()
        {
            if(bundleInfoDict != null)
            {
                foreach (var item in bundleInfoDict)
                {
                    if (item.Value.bundleReference != null)
                    {
                        //item.Value.bundleReference.assetBundle.Unload(true);
                    }
                }
            }
        }

        public void SmartGC()
        {

        }
    }
}


