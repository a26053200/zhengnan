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

        private List<BundleInfo> bundleInfos;
        private Dictionary<string, BundleInfo> bundleInfoDict;
        private Dictionary<string, AssetBundle> assetBundleDict;
        private Dictionary<string, BundleReferenceInfo> bundleReferenceInfoDict;
        private void Awake()
        {
            
        }

        public void LoadBundleData()
        {
            string bundleData = BMUtility.LoadText(getFilePath(BMConfig.BundlDataFile));
            JsonData jsonData = JsonMapper.ToObject(bundleData);
            Debug.Log(bundleData);

            bundleInfos = new List<BundleInfo>();
            bundleInfoDict = new Dictionary<string, BundleInfo>();
            assetBundleDict = new Dictionary<string, AssetBundle>();
            bundleReferenceInfoDict = new Dictionary<string, BundleReferenceInfo>();

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
            BundleReferenceInfo bundleReferenceInfo;
            string path = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
            if (!bundleReferenceInfoDict.TryGetValue(path, out bundleReferenceInfo))
            {
                assetBundle = AssetBundle.LoadFromFile(getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern));
                if (assetBundle == null)
                {
                    Debug.LogErrorFormat("The AssetBundle '{0}' load fail!", path);
                    return null;
                }
                else
                {
                    bundleReferenceInfo = new BundleReferenceInfo()
                    {
                        assetBundle = assetBundle,
                        count = 1,
                        buildType = bundleInfo.buildType,
                        state = BundleLoadState.LoadComplete,
                    };
                    bundleReferenceInfoDict.Add(path, bundleReferenceInfo);
                }
            }
            else
            {
                assetBundle = bundleReferenceInfo.assetBundle;
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
                yield return LoadBundleAsync(bundleInfo, OnAssetBundleLoaded);
        }


        public IEnumerator LoadBundleAsync(BundleInfo bundleInfo, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            BundleReferenceInfo bundleReferenceInfo;
            string path = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
            if (bundleReferenceInfoDict.TryGetValue(path, out bundleReferenceInfo))
            {//就算 缓存池里面有 也要模拟异步加载
                yield return new WaitForEndOfFrame();
                OnAssetBundleLoaded(bundleReferenceInfo.assetBundle);
            }
            else
            {
                bundleReferenceInfo = new BundleReferenceInfo()
                {
                    state = BundleLoadState.Loading,
                    count = 1,
                    buildType = bundleInfo.buildType,
                };
                bundleReferenceInfoDict.Add(path, bundleReferenceInfo);
                AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
                yield return assetBundleCreateRequest;
                if (assetBundleCreateRequest.assetBundle == null)
                {
                    Debug.LogErrorFormat("Failed to load AssetBundle '{0}' !", path);
                }
                bundleReferenceInfo.assetBundle = assetBundleCreateRequest.assetBundle;
                bundleReferenceInfo.state = BundleLoadState.LoadComplete;
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
            BundleReferenceInfo bundleReferenceInfo;
            string path = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
            if (bundleReferenceInfoDict.TryGetValue(path, out bundleReferenceInfo))
                return bundleReferenceInfo.state;
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
            if(assetBundleDict != null)
            {
                foreach (var item in assetBundleDict)
                {
                    item.Value.Unload(true);
                }
            }
        }

        public void SmartGC()
        {

        }
    }
}


