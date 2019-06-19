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

        //输出目录
        static string Output_Path;

        //配置
        static BMSettings settings;

        static List<BuildInfo> buildInfoList;
        
        static string[] ignoreSuffixs; //被忽略的文件后缀
        
        static string[] ignoreFolders; //被忽略的目录

        static double buildStartTime = 0;

        static JsonData buildInfoJson;

        static BuildTarget buildTarget;

        static List<string> tempLuaPaths;

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
            StartBuild(true, BuildTarget.StandaloneWindows64);
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
            tempLuaPaths = new List<string>();

            //加载打包配置
            settings = AssetDatabase.LoadAssetAtPath<BMSettings>(BMSettings_Path);

            buildInfoJson = new JsonData();
            buildInfoJson["resDir"] = settings.resDir;
            buildInfoJson["playform"] = buildTarget.ToString();
            buildInfoJson["bundles"] = new JsonData();

            ignoreSuffixs = settings.Ignore_Suffix.Split(',');
            ignoreFolders = settings.Ignore_Folder.Split(',');

            //Output_Path = Application.dataPath.Replace("Assets", "TestBundle");
            Output_Path = Application.streamingAssetsPath;

            if (Directory.Exists(Output_Path))
                BMEditUtility.DelFolder(Output_Path);
            Directory.CreateDirectory(Output_Path);
            //清空控制台日志
            Debug.ClearDeveloperConsole();
            
            //清除AssetBundleName (这步可以不做)
            RemoveAllAssetBundleName();
            //获取所有Build信息
            FetchAllBuildInfo();
            //计算AssetBundle信息
            CalcAssetBundleInfos();
            //生成AssetBundle
            if(generate)
                GenerateAssetBundle();
            //移动生成后的所有Bundle
            MoveAssetBundle();

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
            
            FetchBuildInfoList(settings.singleFolderList,   settings.singlePattern, settings.scenesCompressType, settings.singleBuildType);
            FetchBuildInfoList(settings.packFolderList,     settings.packPattern,   settings.packCompressType, settings.packBuildType);
            FetchBuildInfoList(settings.scenesFolderList,   settings.scenesPattern, settings.scenesCompressType, settings.scenesBuildType);
            FetchBuildInfoList(settings.shaderFolderList,   settings.shaderPattern, settings.shaderCompressType, settings.shaderBuildType);
            FetchBuildInfoList(settings.luaFolderList, settings.luaPattern, settings.luaCompressType, settings.luaBuildType);
        }

        static void FetchBuildInfoList(List<string> folders, string searchPattern, CompressType compressType, BuildType buildType)
        {
            //包含所有资源的bundle
            for (int i = 0; i < folders.Count; i++)
            {
                if (!Directory.Exists(folders[i]))
                    continue;
                string resDir = BMEditUtility.Relativity2Absolute(folders[i]);
                BuildInfo buildInfo = FetchBuildInfo(resDir, searchPattern, buildType);
                buildInfo.buildName = folders[i];
                buildInfo.buildType = buildType;
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
                    switch(buildInfo.buildType)
                    {
                        case BuildType.Pack:
                            name = BMEditUtility.Path2Name(dirName);
                            break;
                        case BuildType.Scene:
                            name = BMEditUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
                            break;
                        case BuildType.Shader:
                            name = BMEditUtility.Path2Name(buildInfo.buildName);
                            break;
                        case BuildType.Lua:
                            name = BMEditUtility.Path2Name(buildInfo.buildName);
                            break;
                        default:// BuildType.Single:
                            name = BMEditUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
                            break;
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
                                assetBundleVariant = BMConfig.BundleSuffix,
                            },
                            assetPaths = new List<string>(),
                            dependenceMap = new Dictionary<string, string[]>(),
                        };
                        buildInfo.subBuildInfoMap.Add(md5, subInfo);
                    }
                    AddDependence(path, subInfo.dependenceMap);
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
                foreach (var sub in buildInfo.subBuildInfoMap.Values)
                {
                    buildInfoJson["bundles"].Add(sub.ToJson());
                }
            }
            string json = buildInfoJson.ToJson();
            BMEditUtility.SaveUTF8TextFile(Output_Path + "/" +BMConfig.BundlDataFile, JsonFormatter.PrettyPrint(json));

            Logger.Log("Calc Asset Bundle Infos Over.");
        }

        static void GenerateAssetBundle()
        {
            int count = 0;
            Dictionary<BuildAssetBundleOptions, List<AssetBundleBuild>> buildAbbMap = new Dictionary<BuildAssetBundleOptions, List<AssetBundleBuild>>(); 
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                foreach (var subInfo in buildInfo.subBuildInfoMap.Values)
                {
                    for (int j = 0; j < subInfo.assetPaths.Count; j++)
                    {
                        if(BuildType.Scene == buildInfo.buildType)
                        {
                            string path = subInfo.assetPaths[j];
                            string dirName = Path.GetDirectoryName(path);
                            string name = BMEditUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
                            string outputPath = string.Format("{0}/{1}.{2}", Output_Path, name, BMConfig.BundleSuffix);
                            BuildPipeline.BuildPlayer(new string[] { path },
                                outputPath, buildTarget,
                                BuildOptions.BuildAdditionalStreamedScenes);
                            count += 1;
                        }
                        else
                        {
                            BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;
                            switch (buildInfo.compressType)
                            {
                                case CompressType.LZ4:
                                    opt |= BuildAssetBundleOptions.ChunkBasedCompression;
                                    break;
                                case CompressType.None:
                                    opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
                                    break;
                            }
                            switch (buildInfo.buildType)
                            {
                                case BuildType.Pack:
                                case BuildType.Single:
                                case BuildType.Shader:
                                    opt |= BuildAssetBundleOptions.DeterministicAssetBundle;
                                    break;
                                case BuildType.Lua:
                                    opt |= BuildAssetBundleOptions.DeterministicAssetBundle;
                                    break;
                            }

                            List<AssetBundleBuild> list;
                            if (!buildAbbMap.TryGetValue(opt, out list))
                            {
                                list = new List<AssetBundleBuild>();
                                buildAbbMap.Add(opt, list);
                            }
                            if (!list.Contains(subInfo.assetBundleBuild))
                                list.Add(subInfo.assetBundleBuild);
                        }
                        
                    }
                }
                
            }
            foreach (var buildAbb in buildAbbMap)
            {
                count += buildAbb.Value.Count;
                BuildPipeline.BuildAssetBundles(Output_Path, buildAbb.Value.ToArray(), buildAbb.Key, buildTarget);
            }
            foreach (var path in tempLuaPaths)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            AssetDatabase.Refresh();
            Logger.Log("Generate Assets Bundle Over. num:{0} time consuming:{1}s", count, EditorApplication.timeSinceStartup - buildStartTime);
        }

        
        static void MoveAssetBundle()
        {
            if(buildTarget == BuildTarget.StandaloneWindows64)
            {
                Logger.Log("MoveTo:{0}", Application.persistentDataPath);
                //BMEditUtility.CopyDir(Output_Path, Application.persistentDataPath);
            }
            else
            {

            }
        }

        //=======================
        // 工具函数
        //=======================

        //获取Build信息
        static BuildInfo FetchBuildInfo(string resDir, string searchPattern, BuildType buildType)
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
                if (buildType == BuildType.Lua)
                {
                    string temp = path + "." + StringUtils.EncryptWithMD5(path) + ".txt";
                    if (File.Exists(temp))
                        File.Delete(temp);
                    File.Copy(path, temp);
                    buildInfo.assetPaths.Add(temp);
                    tempLuaPaths.Add(temp);
                }
                else
                {
                    buildInfo.assetPaths.Add(path);
                }
                //Logger.Log("path:{0}", path);
                EditorUtility.DisplayProgressBar("Fetch Build Info...", path, (float)(i + 1.0f) / (float)resFiles.Length);
            }
            //EditorUtility.ClearProgressBar();
            if (buildType == BuildType.Lua)
            {
                AssetDatabase.Refresh();
            }
            return buildInfo;
        }

        //忽略文件
        static bool IgnoreFile(string lowerName, string dirPath)
        {
            for (int i = 0; i < ignoreSuffixs.Length; i++)
            {
                if (lowerName.EndsWith(ignoreSuffixs[i]))
                    return true;
            }
            for (int i = 0; i < ignoreFolders.Length; i++)
            {
                if (dirPath.IndexOf(ignoreFolders[i]) != -1)
                    return true;
            }
            return false;
        }

        static void AddDependence(string filePath, Dictionary<string, string[]> dependenceMap)
        {
            bool owner = false;
            for (int i = 0; i < settings.ownerDependenceSuffixs.Count; i++)
            {
                if (filePath.EndsWith(settings.ownerDependenceSuffixs[i]))
                {
                    owner = true;
                    break;
                }
            }
            if(owner)
            {
                string[] dependencePaths = AssetDatabase.GetDependencies(filePath, true);
                dependenceMap[filePath] = dependencePaths;
            }
        }
    }
}
    