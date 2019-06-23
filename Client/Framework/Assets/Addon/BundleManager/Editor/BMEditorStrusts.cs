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
        public BuildType buildType;             //打包类型
        public CompressType compressType;       //压缩类型
        public List<string> assetPaths;
        public Dictionary<string, SubBuildInfo> subBuildInfoMap;

    }
    public class SubBuildInfo
    {
        public string bundleName;
        public string buildMd5;
        public BuildType buildType;
        public uint crc;
        public long size;
        public List<string> assetPaths;
        public AssetBundleBuild assetBundleBuild;
        public Dictionary<string, string[]> dependenceMap;

        public JsonData ToJson()
        {
            JsonData json = new JsonData();
            json["bundleName"] = bundleName;
            json["buildMd5"] = buildMd5;
            json["buildType"] = buildType.ToString();
            json["crc"] = crc;
            json["size"] = size;
            json["assetPaths"] = new JsonData();
            for (int i = 0; i < assetPaths.Count; i++)
            {
                json["assetPaths"].Add(assetPaths[i]);
            }
            if(dependenceMap.Count > 0)
            {
                //json["dependenceMap"] = new JsonData();
                foreach (var dep in dependenceMap)
                {
                    JsonData depJson = new JsonData();
                    //depJson["path"] = dep.Key;
                    json["dependencePaths"] = depJson;
                    for (int i = 0; i < dep.Value.Length; i++)
                    {
                        depJson.Add(dep.Value[i]);
                    }
                    //json["dependenceMap"].Add(depJson);
                }
            }
            return json;
        }
    }

}


