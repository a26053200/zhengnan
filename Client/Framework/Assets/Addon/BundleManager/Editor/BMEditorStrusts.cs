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
        public List<AssetBundleBuild> assetBundleBuilds;
    }

}


