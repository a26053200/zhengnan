using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BM
{
    /// <summary>
    /// <para>Bundle Builder</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 0:24:07</para>
    /// </summary> 
    public static class BundleBuilder
    {
        //配置路径
        static string BMSettings_Path = "Assets/Res/BMSettings.asset";

        static string Output_Path;

        //配置
        static BMSettings settings;

        static List<BuildInfo> buildInfoList;

        static double buildStartTime = 0;

        static JsonData buildInfoJson;

        static BuildTarget buildTarget;

        [MenuItem("Window/Bundle Manager(IOS)")]
        public static void BuildIOS()
        {
            StartBuild(true, BuildTarget.iOS);
        }

        [MenuItem("Window/Bundle Manager(Android)")]
        public static void BuildAndroid()
        {
            StartBuild(true, BuildTarget.Android);
        }

        [MenuItem("Window/Bundle Manager(Win64)")]
        public static void BuildWin64()
        {
            StartBuild(true, BuildTarget.iOS);
        }

        [MenuItem("Window/Bundle Manager_Test")]
        public static void Test()
        {
            StartBuild(false,BuildTarget.StandaloneWindows64);
        }
        //=======================
        // 流程函数
        //=======================

        public static List<BuildInfo> StartBuild(bool generate,BuildTarget _buildTarget)
        {
            buildTarget = _buildTarget;
            buildStartTime = EditorApplication.timeSinceStartup;

            buildInfoJson = new JsonData();

            Output_Path = Application.dataPath.Replace("Assets", "TestBundle");
            if (Directory.Exists(Output_Path))
                BMEditUtility.DelFolder(Output_Path);
            Directory.CreateDirectory(Output_Path);
            //清空控制台日志
            Debug.ClearDeveloperConsole();
            //加载打包配置
            settings = AssetDatabase.LoadAssetAtPath<BMSettings>(BMSettings_Path);

            //清除AssetBundleName (这步可以不做)
            RemoveAllAssetBundleName();
            //获取所有Build信息
            FetchAllBuildInfo();
            //计算AssetBundle信息
            CalcAssetBundleInfos();
            //生成AssetBundle
            if(generate)
                GenerateAssetBundle();

            return buildInfoList;
        }

        static void RemoveAllAssetBundleName()
        {
            string[] names = AssetDatabase.GetAllAssetBundleNames();
            foreach (string name in names)
                AssetDatabase.RemoveAssetBundleName(name, true);
        }

        static void FetchAllBuildInfo()
        {
            buildInfoList = new List<BuildInfo>();

            FetchBuildInfoList(settings.bundleFolderList, settings.bundlePattern, settings.bundleCompressType);
            FetchBuildInfoList(settings.packFolderList, settings.packPattern, settings.packCompressType,true);
            FetchBuildInfoList(settings.scenesFolderList, settings.scenesPattern, settings.scenesCompressType, false, true);
            FetchBuildInfoList(settings.completeFolderList, settings.completePattern, settings.completeCompressType, false, false, true);
        }

        static void FetchBuildInfoList(List<string> folders, string searchPattern, CompressType compressType, bool isPack = false, bool isScene = false, bool isCompleteAssets = false)
        {
            //包含所有资源的bundle
            for (int i = 0; i < folders.Count; i++)
            {
                if (!Directory.Exists(folders[i]))
                    continue;
                string resDir = BMEditUtility.Relativity2Absolute(folders[i]);
                BuildInfo buildInfo = FetchBuildInfo(resDir, searchPattern);
                buildInfo.buildName = folders[i];
                buildInfo.isCompleteAssets = isCompleteAssets;
                buildInfo.isPack = isPack;
                buildInfo.isScene = isScene;
                buildInfo.compressType = compressType;
                buildInfoList.Add(buildInfo);
            }
        }

        static void CalcAssetBundleInfos()
        {
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                buildInfo.subBuildInfoMap = new Dictionary<string, SubBuildInfo>();
                for (int j = 0; j < buildInfo.assetPaths.Count; j++)
                {
                    string path = buildInfo.assetPaths[j];
                    string dirName = Path.GetDirectoryName(path);
                    string name;
                    if (buildInfo.isPack)//整包
                    {
                        name = BMEditUtility.Path2Name(dirName);
                    }
                    else if (buildInfo.isScene)//场景包
                    {
                        name = BMEditUtility.Path2Name(dirName + "/" +Path.GetFileNameWithoutExtension(path));
                    }
                    else//一般
                    {
                        name = BMEditUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
                    }
                    string md5 = StringUtils.EncryptWithMD5(name);
                    SubBuildInfo subInfo = null;
                    if (!buildInfo.subBuildInfoMap.TryGetValue(md5, out subInfo))
                    {
                        subInfo = new SubBuildInfo()
                        {
                            bundleName = name,
                            buildMd5 = md5,
                            assetBundleBuild = new AssetBundleBuild()
                            {
                                assetBundleName = name,
                                assetBundleVariant = settings.Suffix_Bundle,
                            },
                            assetPaths = new List<string>(),
                            dependenceMap = new Dictionary<string, string[]>(),
                        };
                        buildInfo.subBuildInfoMap.Add(md5, subInfo);
                    }
                    string[] dependencePaths;
                    if (!subInfo.dependenceMap.TryGetValue(path, out dependencePaths))
                    {
                        dependencePaths = AssetDatabase.GetDependencies(path, true);
                        subInfo.dependenceMap.Add(path, dependencePaths);
                    }
                    subInfo.assetPaths.Add(path);
                    AssetBundleBuild abb = subInfo.assetBundleBuild;
                    abb.assetNames = subInfo.assetPaths.ToArray();
                    //abb.addressableNames = subInfo.assetPaths.ToArray();
                    subInfo.assetBundleBuild = abb;
                    EditorUtility.DisplayProgressBar("Calc Asset Bundle Build Infos...", path, (float)(j + 1.0f) / (float)buildInfo.assetPaths.Count);
                }
            }
            EditorUtility.ClearProgressBar();
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                buildInfoJson.Add(buildInfo.ToJson());
            }
            string json = buildInfoJson.ToJson();
            BMEditUtility.SaveUTF8TextFile(Output_Path + "/BundleData.json", JsonFormatter.PrettyPrint(json));

            Logger.Log("Calc Asset Bundle Infos Over.");
        }

        static void GenerateAssetBundle()
        {
            int count = 0;
            Dictionary<CompressType, List<AssetBundleBuild>> buildAbbMap = new Dictionary<CompressType, List<AssetBundleBuild>>(); 
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                foreach (var subInfo in buildInfo.subBuildInfoMap.Values)
                {
                    for (int j = 0; j < subInfo.assetPaths.Count; j++)
                    {
                        if (buildInfo.isScene)
                        {
                            string path = subInfo.assetPaths[j];
                            string dirName = Path.GetDirectoryName(path);
                            string name = Path.GetFileNameWithoutExtension(path);
                            string outputPath = string.Format("{0}/{1}.{2}", Output_Path, name, settings.Suffix_Bundle);
                            BuildPipeline.BuildPlayer(new string[] { path },
                                outputPath, buildTarget,
                                BuildOptions.BuildAdditionalStreamedScenes);
                                count += 1;
                        }
                        else
                        {
                            List<AssetBundleBuild> list;
                            if(!buildAbbMap.TryGetValue(buildInfo.compressType, out list))
                            {
                                list = new List<AssetBundleBuild>();
                                buildAbbMap.Add(buildInfo.compressType, list);
                            }
                            if(!list.Contains(subInfo.assetBundleBuild))
                                list.Add(subInfo.assetBundleBuild);
                        }
                    }
                }
                
            }

            foreach(var buildAbb in buildAbbMap)
            {
                BuildAssetBundleOptions bbOpt = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
                switch (buildAbb.Key)
                {
                    case CompressType.LZ4:
                        bbOpt = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
                        break;
                    case CompressType.LZMA:
                        bbOpt = BuildAssetBundleOptions.DeterministicAssetBundle;
                        break;
                    case CompressType.None:
                        bbOpt = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle;
                        break;
                }
                BuildPipeline.BuildAssetBundles(Output_Path, buildAbb.Value.ToArray(), bbOpt, buildTarget);
            }
            count += buildAbbMap.Count;
            Logger.Log("Generate Assets Bundle Over. num:{0} time consuming:{1}s", count, EditorApplication.timeSinceStartup - buildStartTime);
        }

        //=======================
        // 工具函数
        //=======================

        //获取Build信息
        static BuildInfo FetchBuildInfo(string resDir, string searchPattern)
        {
            BuildInfo buildInfo = new BuildInfo();
            buildInfo.assetPaths = new List<string>();
            FileInfo[] resFiles = BMEditUtility.GetAllFiles(resDir, searchPattern);
            for (int i = 0; i < resFiles.Length; i++)
            {
                FileInfo fileInfo = resFiles[i];
                string lowerName = fileInfo.Name.ToLower();
                string dirPath = BMEditUtility.Absolute2Relativity(fileInfo.DirectoryName) + "/";

                //过滤一些文件
                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if ((fileInfo.Directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if (IgnoreFile(lowerName, dirPath))
                    continue;

                string path = dirPath + fileInfo.Name; //相对路径
                buildInfo.assetPaths.Add(path);
                //Logger.Log("path:{0}", path);
                EditorUtility.DisplayProgressBar("Fetch Build Info...", path, (float)(i + 1.0f) / (float)resFiles.Length);
            }
            //EditorUtility.ClearProgressBar();
            return buildInfo;
        }

        //忽略文件
        static bool IgnoreFile(string lowerName, string dirPath)
        {
            string[] ignoreSuffixs = settings.Ignore_Suffix.Split(',');
            for (int i = 0; i < ignoreSuffixs.Length; i++)
            {
                if (lowerName.EndsWith(ignoreSuffixs[i]))
                    return true;
            }
            string[] ignoreFolders = settings.Ignore_Folder.Split(',');
            for (int i = 0; i < ignoreFolders.Length; i++)
            {
                if (dirPath.IndexOf(ignoreFolders[i]) != -1)
                    return true;
            }
            return false;
        }
    }
}
    