using LitJson;
using System;
using System.Linq;
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
        //打包版本记录
        const string BM_Build_Version = "BM_Build_Version";

        //配置
        public static BMSettings settings;

        //历史记录
        public static Dictionary<string, BuildSampleInfo> historyBuildInfo;

        //历史记录文库路径
        public static string historyBuildInfoPath;

        //输出目录
        public static string Output_Root_Path;

        //输出目录
        public static string Output_Path;

        
        //AssetBundleName 对应资源的映射
        static string AssetBundleNameMap_Path;

        //版本
        static Version version;

        static List<BuildInfo> buildInfoList;
        
        static string[] ignoreSuffixs; //被忽略的文件后缀
        
        static string[] ignoreFolders; //被忽略的目录

        static double buildStartTime = 0;

        static JsonData buildInfoJson;

        static BuildTarget buildTarget;

        static List<string> tempLuaPaths;

        //=======================
        // 流程函数
        //=======================

        public static List<BuildInfo> StartBuild(bool isForceBuild, Language language, BuildTarget _buildTarget, bool generate)
        {
            buildTarget = _buildTarget;
            buildStartTime = EditorApplication.timeSinceStartup;
            tempLuaPaths = new List<string>();

            //记录打包次数
            string verStr = EditorPrefs.GetString(settings.AppName + BM_Build_Version, null);
            if (string.IsNullOrEmpty(verStr))
                version = new Version(0,0,0,1);
            else
            {
                version = new Version(verStr);
                version = new Version(version.Major, version.Minor, version.Build, version.Revision + 1);
            }

            buildInfoJson = new JsonData();
            buildInfoJson["resDir"] = settings.resDir;
            buildInfoJson["platform"] = buildTarget.ToString();
            buildInfoJson["bundles"] = new JsonData();

            ignoreSuffixs = settings.Ignore_Suffix.Split(',');
            ignoreFolders = settings.Ignore_Folder.Split(',');

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
            //做增量过滤
            IncrementFilter();
            //生成AssetBundle
            if(generate)
                GenerateAssetBundle();
            //移动生成后的所有Bundle
            if (!isForceBuild)
                MoveAssetBundle();
            //计算Bundle文件大小
            if (generate)
                CalcBundleFileSize();
            //写入Bundle信息
            SaveBundleInfo();

            Logger.Log("Generate Assets Bundle Over. time consuming:{0}s", EditorApplication.timeSinceStartup - buildStartTime);
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
            
            FetchBuildInfoList(settings.singleFolderList,   settings.singlePattern, settings.singleCompressType, settings.singleBuildType);
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
                    switch (buildInfo.buildType)
                    {
                        case BuildType.Pack:
                            name = BMUtility.Path2Name(dirName);
                            break;
                        case BuildType.Scene:
                            name = BMUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
                            break;
                        case BuildType.Shader:
                            name = BMUtility.Path2Name(buildInfo.buildName);
                            break;
                        case BuildType.Lua:
                            name = BMUtility.Path2Name(buildInfo.buildName);
                            break;
                        default:// BuildType.Single:
                            name = BMUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
                            break;
                    }
                    string md5 = BMUtility.EncryptWithMD5(name);
                    SubBuildInfo subInfo = null;
                    if (!buildInfo.subBuildInfoMap.TryGetValue(md5, out subInfo))
                    {
                        subInfo = new SubBuildInfo()
                        {
                            bundleName = name,
                            buildMd5 = md5,
                            buildType = buildInfo.buildType,
                            assetBundleBuild = new AssetBundleBuild()
                            {
                                assetBundleName = name,
                                assetBundleVariant = BMConfig.BundleSuffix,
                            },
                            assetPaths = new List<string>(),
                            assetHashs = new List<string>(),
                            dependenceMap = new Dictionary<string, string[]>(),
                        };
                        buildInfo.subBuildInfoMap.Add(md5, subInfo);
                    }
                    //添加依赖关系
                    AddDependence(path, subInfo.dependenceMap);
                    string hash = HashHelper.ComputeMD5(path);
                    subInfo.assetPaths.Add(path.ToLower());
                    subInfo.assetHashs.Add(hash);
                    AssetBundleBuild abb = subInfo.assetBundleBuild;
                    abb.assetNames = subInfo.assetPaths.ToArray();
                    
                    //abb.addressableNames = subInfo.assetPaths.ToArray();
                    subInfo.assetBundleBuild = abb;
                    EditorUtility.DisplayProgressBar("Calc Asset Bundle Build Infos...", path, (float)(j + 1.0f) / (float)buildInfo.assetPaths.Count);
                }
            }
            EditorUtility.ClearProgressBar();
            

            Logger.Log("Calc Asset Bundle Infos Over.");
        }
        //增量过滤
        static void IncrementFilter()
        {
            int newCount = 0, forceCount = 0, filterCount = 0, total = 0;
            if (historyBuildInfo!=null)
            {
                for (int i = 0; i < buildInfoList.Count; i++)
                {
                    BuildInfo buildInfo = buildInfoList[i];
                    foreach (var subInfo in buildInfo.subBuildInfoMap.Values)
                    {
                        total++;
                        historyBuildInfo.TryGetValue(subInfo.bundleName, out BuildSampleInfo historyInfo);
                        if (historyInfo == null)
                        {//全新的bundle
                            newCount++;
                            continue;
                        }
                        EditorUtility.DisplayProgressBar("Increment Filter Asset Bundle...", subInfo.bundleName, (float)(i + 1.0f) / (float)buildInfoList.Count);
                        if (BuildType.Scene != subInfo.buildType && BuildType.Lua != subInfo.buildType)
                        {//非强制打包需要做增量过滤
                            if (historyInfo.assetPaths.Count != subInfo.assetPaths.Count)
                            {
                                newCount++;
                                continue;
                            }
                            if (!historyInfo.assetPaths.SequenceEqual(subInfo.assetPaths))
                            {
                                newCount++;
                                continue;
                            }
                            if (!historyInfo.assetHashs.SequenceEqual(subInfo.assetHashs))
                            {
                                newCount++;
                                continue;
                            }
                            filterCount++;
                            subInfo.ignore = true;
                        }
                        else
                        {
                            forceCount++;
                        }
                    }
                }
            }
            Logger.Log(string.Format("Build new bundle {0}, Increment filter {1}, Force Build {2}, Total {3}", newCount, filterCount, forceCount, total));
            EditorUtility.ClearProgressBar();
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
                        if (subInfo.ignore)
                        {
                            continue;//增量更新被忽略掉
                        }
                        if(BuildType.Scene == buildInfo.buildType)
                        {
                            string path = subInfo.assetPaths[j];
                            string dirName = Path.GetDirectoryName(path);
                            string name = BMUtility.Path2Name(dirName + "/" + Path.GetFileNameWithoutExtension(path));
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
            
        }

        
        static void MoveAssetBundle()
        {
            if(buildTarget == BuildTarget.StandaloneWindows64)
            {
                //Logger.Log("MoveTo:{0}", Application.persistentDataPath);
                //BMEditUtility.CopyDir(Output_Path, Application.persistentDataPath);
            }
            else
            {

            }
        }

        static void CalcBundleFileSize()
        {
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                
                foreach (var subInfo in buildInfo.subBuildInfoMap.Values)
                {
                    string path = Path.Combine(Output_Path, subInfo.bundleName + BMConfig.BundlePattern);
                    subInfo.size = new FileInfo(path).Length;
                    uint crc;
                    BuildPipeline.GetCRCForAssetBundle(path, out crc);
                    subInfo.crc = crc;
                    AssetBundle ab = AssetBundle.LoadFromFile(path);
                    EditorUtility.DisplayProgressBar("Calc Bundle Size...", path, (float)(i + 1.0f) / (float)buildInfoList.Count);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        static void SaveBundleInfo()
        {
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                foreach (var sub in buildInfo.subBuildInfoMap.Values)
                {
                    buildInfoJson["bundles"].Add(sub.ToJson());
                }
            }
            string json = buildInfoJson.ToJson();
            BMEditUtility.SaveUTF8TextFile(Output_Path + "/" + BMConfig.BundlDataFile, JsonFormatter.PrettyPrint(json));
            BMEditUtility.SaveUTF8TextFile(Output_Path + "/" + BMConfig.VersionFile, version.ToString());
            //BMEditUtility.SaveDictionary(historyBuildInfoPath, historyBuildInfoPath);
            EditorPrefs.SetString(settings.AppName + BM_Build_Version, version.ToString());
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
                    string temp = path + "." + BMUtility.EncryptWithMD5(path) + ".txt";
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
                List<string> list = new List<string>(dependencePaths);
                list.Remove(filePath);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = list[i].ToLower();
                }
                if(list.Count > 0)
                    dependenceMap[filePath] = list.ToArray();
            }
        }
    }
}
    