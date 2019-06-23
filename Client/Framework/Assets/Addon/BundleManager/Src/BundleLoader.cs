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
            return LoadBundleSync(bundleInfo.bundleName);
        }

        public AssetBundle LoadBundleSync(string bundleName)
        {
            AssetBundle assetBundle;
            string path = getFilePath(bundleName + BMConfig.BundlePattern);
            if (!assetBundleDict.TryGetValue(path, out assetBundle))
                assetBundle = AssetBundle.LoadFromFile(getFilePath(bundleName + BMConfig.BundlePattern));
            if (assetBundle == null)
            {
                Debug.LogErrorFormat("The AssetBundle '{0}' load fail!", path);
                return null;
            }
            Debug.LogFormat("The AssetBundle '{0}' load success!", path);
            return assetBundle;
        }

        //=======================
        // 异步加载
        //=======================

        public IEnumerator LoadAssetBundleAsync(string bundleName, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            BundleInfo bundleInfo = GetBundleInfo(bundleName);
            if(bundleInfo != null)
                yield return LoadBundleAsync(bundleInfo.bundleName, OnAssetBundleLoaded);
        }


        public IEnumerator LoadBundleAsync(string bundleName, UnityAction<AssetBundle> OnAssetBundleLoaded)
        {
            AssetBundle assetBundle;
            string path = getFilePath(bundleName + BMConfig.BundlePattern);
            if (assetBundleDict.TryGetValue(path, out assetBundle))
            {//就算 缓存池里面有 也要模拟异步加载
                yield return new WaitForEndOfFrame();
                OnAssetBundleLoaded(assetBundle);
            }
            else
            {
                AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
                yield return assetBundleCreateRequest;
                if (assetBundleCreateRequest.assetBundle == null)
                {
                    Debug.LogErrorFormat("Failed to load AssetBundle '{0}' !", path);
                }
                OnAssetBundleLoaded(assetBundleCreateRequest.assetBundle);
            }
        }
        
        //同步加载依赖

        //异步加载依赖

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
    }
}


