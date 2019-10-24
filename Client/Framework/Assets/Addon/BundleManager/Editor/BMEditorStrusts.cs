using LitJson;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.TestTools.Constraints;

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
        public int version;                    //版本号
        public List<string> assetPaths;
        public Dictionary<string, SubBuildInfo> subBuildInfoMap;

    }
    public class BuildSampleInfo
    {
        public string bundleName;
        public int version;
        public List<string> assetPaths;
        public List<string> assetHashs;
    }
    public class SubBuildInfo
    {
        public string bundleName;
        public string buildMd5;
        public BuildType buildType;
        public int version;
        public uint crc;
        public long size;
        public List<string> assetPaths;
        public List<string> assetHashs;
        public AssetBundleBuild assetBundleBuild;
        public Dictionary<string, string[]> dependenceMap;
        public Dictionary<string, string[]> dependenceHashMap;

        public bool ignore = false;
        public JsonData ToJson()
        {
            JsonData json = new JsonData();
            json["bundleName"] = bundleName;
            json["buildMd5"] = buildMd5;
            json["buildType"] = buildType.ToString();
            json["version"] = version;
            json["crc"] = crc;
            json["size"] = size;
            json["assetPaths"] = new JsonData();
            for (int i = 0; i < assetPaths.Count; i++)
            {
                json["assetPaths"].Add(assetPaths[i]);
            }
            json["assetHashs"] = new JsonData();
            for (int i = 0; i < assetPaths.Count; i++)
            {
                json["assetHashs"].Add(assetHashs[i]);
            }
            if (dependenceMap.Count > 0)
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
            if (dependenceHashMap.Count > 0)
            {
                //json["dependenceHashs"] = new JsonData();
                foreach (var dep in dependenceHashMap)
                {
                    JsonData depJson = new JsonData();
                    //depJson["path"] = dep.Key;
                    json["dependenceHashs"] = depJson;
                    for (int i = 0; i < dep.Value.Length; i++)
                    {
                        depJson.Add(dep.Value[i]);
                    }
                    //json["dependenceHashs"].Add(depJson);
                }
            }
            return json;
        }
    }

}


