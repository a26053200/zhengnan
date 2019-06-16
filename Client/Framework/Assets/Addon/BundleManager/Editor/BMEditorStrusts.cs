using LitJson;
using System.Collections.Generic;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>打包信息</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/14 22:07:09</para>
    /// </summary> 
    public class BuildInfo
    {
        public string buildName;
        public bool isCompleteAssets;           //打包整个文件夹
        public bool isScene;                    //是否是场景
        public bool isPack;                    //打包每个子目录
        public CompressType compressType;       //压缩类型
        public List<string> assetPaths;
        public Dictionary<string, SubBuildInfo> subBuildInfoMap;

        public JsonData ToJson()
        {
            JsonData json = new JsonData();
            json["buildName"] = buildName;
            json["subBuildInfoMap"] = new JsonData();
            foreach(var subInfo in subBuildInfoMap.Values)
            {
                json["subBuildInfoMap"].Add(subInfo.ToJson());
            }

            return json;
        }
    }
    public class SubBuildInfo
    {
        public string bundleName;
        public string buildMd5;
        public List<string> assetPaths;
        public AssetBundleBuild assetBundleBuild;
        public Dictionary<string, string[]> dependenceMap;

        public JsonData ToJson()
        {
            JsonData json = new JsonData();
            json["bundleName"] = bundleName;
            json["buildMd5"] = buildMd5;
            json["assetPaths"] = new JsonData();
            for (int i = 0; i < assetPaths.Count; i++)
            {
                json["assetPaths"].Add(assetPaths[i]);
            }
            //json["dependenceMap"] = new JsonData();
            //bool hasDependence = false;
            //foreach (var dep in dependenceMap)
            //{
            //    JsonData depJson = new JsonData();
            //    depJson["path"] = dep.Key;
            //    depJson["dependencePaths"] = new JsonData();
            //    for (int i = 0; i < dep.Value.Length; i++)
            //    {
            //        depJson["dependencePaths"].Add(dep.Value[i]);
            //        hasDependence = true;
            //    }
            //    json["dependenceMap"].Add(depJson);
            //}
            //if (!hasDependence)
            //{
            //    json.Keys.Remove("dependenceMap");
            //}
            return json;
        }
    }

}


