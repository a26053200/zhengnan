using System;
using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

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

        public AssetBundle LoadAssetBundle(string assetPath)
        {
            BundleInfo bundleInfo;
            if(bundleInfoDict.TryGetValue(assetPath, out bundleInfo))
            {
                string path = getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern);
                AssetBundle assetBundle;
                if (assetBundleDict.TryGetValue(path, out assetBundle))
                {

                }
                else
                {
                    assetBundle = AssetBundle.LoadFromFile(getFilePath(bundleInfo.bundleName + BMConfig.BundlePattern));
                    if (assetBundle == null)
                    {
                        Debug.LogErrorFormat("The AssetBundle '{0}' load fail!", assetPath);
                        return null;
                    }
                    assetBundleDict.Add(path, assetBundle);
                }
                return assetBundle;
            }
            else
            {
                Debug.LogErrorFormat("No path is '{0}' bundle.", assetPath);
            }
            return null;
        }

        private string getFilePath(string fileName)
        {
            string path = Path.Combine(BMConfig.RawDir, fileName);
            if (!BMUtility.FileExists(path))
                path = Path.Combine(BMConfig.ReadonlyDir, fileName);
            return path;
        }

        private void OnDestroy()
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


