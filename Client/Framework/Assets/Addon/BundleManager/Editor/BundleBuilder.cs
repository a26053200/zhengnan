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

        static bool isForceBuild;

        static bool isLuaEncoded;

        //=======================
        // 流程函数
        //=======================

        public static void StartBuild(bool forceBuild, Language language, BuildTarget buildTag, bool generate, bool luaEncoded, bool moveBundle)
        {
            buildTarget = buildTag;
            buildStartTime = EditorApplication.timeSinceStartup;
            tempLuaPaths = new List<string>();

            isForceBuild = forceBuild;
            isLuaEncoded = luaEncoded;

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
            buildInfoJson["suffix"] = settings.Suffix_Bundle;
            buildInfoJson["useHashName"] = settings.useHashName;
            buildInfoJson["language"] = language.ToString();
            buildInfoJson["bundles"] = new JsonData();

            ignoreSuffixs = settings.Ignore_Suffix.Split(',');
            ignoreFolders = settings.Ignore_Folder.Split(',');

            //强制打包时情况原来的目录
            if (isForceBuild && Directory.Exists(Output_Path))
                BMEditUtility.DelFolder(Output_Path);
            Directory.CreateDirectory(Output_Path);
            //清空控制台日志
            Debug.ClearDeveloperConsole();
            
            //清除AssetBundleName (这步可以不做)
            //RemoveAllAssetBundleName();
            //获取所有Build信息
            FetchAllBuildInfo();
            //计算AssetBundle信息
            CalcAssetBundleInfos();
            //做增量过滤
            if (!isForceBuild)//是否是强制重新Build
                IncrementFilter();
            //生成AssetBundle
            if(generate)
                GenerateAssetBundle();
            
            //计算Bundle文件大小
            if (generate)
                CalcBundleFileSize();
            //写入Bundle信息
            SaveBundleInfo();
            
            //清除manifest文件
            if (settings.clearManifestFile)
                ClearManifestFiles();

            Logger.Log("Generate Assets Bundle Over. time consuming:{0}s", EditorApplication.timeSinceStartup - buildStartTime);
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
                            int index = settings.scenePaths.IndexOf(path);
                            if (index == -1)
                            {//新场景
                                buildInfo.version = 1;
                                settings.scenePaths.Add(path);
                                settings.sceneVersions.Add(1);
                            }
                            else
                                buildInfo.version = settings.sceneVersions[index];
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
                        string abName = name;
                        if (settings.useHashName)
                            abName = md5;
                        subInfo = new SubBuildInfo()
                        {
                            bundleName = name,
                            buildMd5 = md5,
                            buildType = buildInfo.buildType,
                            version = buildInfo.version,
                            assetBundleBuild = new AssetBundleBuild()
                            {
                                assetBundleName = abName,
                                assetBundleVariant = settings.Suffix_Bundle,
                            },
                            assetPaths = new List<string>(),
                            assetHashs = new List<string>(),
                            dependenceMap = new Dictionary<string, string[]>(),
                            dependenceHashMap = new Dictionary<string, string[]>(),
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
                        if (BuildType.Lua != subInfo.buildType)
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
                            if (BuildType.Scene == subInfo.buildType)
                            {
                                if (buildInfo.version > historyInfo.version)
                                {
                                    newCount++;
                                    continue;
                                }
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
                            if (settings.useHashName)
                                name = subInfo.buildMd5;
                            string outputPath = string.Format("{0}/{1}.{2}", Output_Path, name, settings.Suffix_Bundle);
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
            //清除临时的lua bundle 文件
            foreach (var path in tempLuaPaths)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            
            AssetDatabase.Refresh();
        }
        
        static void ClearManifestFiles()
        {
            FileInfo[] manifestFiles = BMEditUtility.GetAllFiles(Output_Path, "*.manifest");
            for (int i = 0; i < manifestFiles.Length; i++)
            {
                FileInfo fileInfo = manifestFiles[i];
                string path = fileInfo.DirectoryName + "/" + fileInfo.Name; //相对路径
                Debug.Log(path);
                if(File.Exists(path))
                    File.Delete(path);
                EditorUtility.DisplayProgressBar("Clear Manifest Files...", path, (float)(i + 1.0f) / (float)manifestFiles.Length);
            }
            EditorUtility.ClearProgressBar();
        }

        static void CalcBundleFileSize()
        {
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                //增量更新时已经被删除的bundle
                List<string> hasDeletedBundleList = new List<string>();
                foreach (var key in buildInfo.subBuildInfoMap.Keys)
                {
                    var subInfo = buildInfo.subBuildInfoMap[key];
                    string path = Path.Combine(Output_Path, (settings.useHashName ? subInfo.buildMd5: subInfo.bundleName) + "." + settings.Suffix_Bundle);
                    if (!File.Exists(path))
                    {
                        hasDeletedBundleList.Add(key);
                        continue;
                    }
                    subInfo.size = new FileInfo(path).Length;
                    uint crc;
                    BuildPipeline.GetCRCForAssetBundle(path, out crc);
                    subInfo.crc = crc;
                    //AssetBundle ab = AssetBundle.LoadFromFile(path);
                    EditorUtility.DisplayProgressBar("Calc Bundle Size...", path, (float)(i + 1.0f) / (float)buildInfoList.Count);
                }

                foreach (var delKey in hasDeletedBundleList)
                {
                    var subInfo = buildInfo.subBuildInfoMap[delKey];
                    string path = Path.Combine(Output_Path, (settings.useHashName ? subInfo.buildMd5: subInfo.bundleName) + "." + settings.Suffix_Bundle);
                    buildInfo.subBuildInfoMap.Remove(delKey);
                    Debug.LogFormat("Delete bundle {0}", path);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        static void SaveBundleInfo()
        {
            int total = 0;
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                foreach (var sub in buildInfo.subBuildInfoMap.Values)
                {
                    total++;
                    buildInfoJson["bundles"].Add(sub.ToJson());
                }
            }
            string json = buildInfoJson.ToJson();
            BMEditUtility.SaveUTF8TextFile(Output_Path + "/" + BMConfig.BundleDataFile, JsonFormatter.PrettyPrint(json));
            BMEditUtility.SaveUTF8TextFile(Output_Path + "/" + BMConfig.VersionFile, version.ToString());
            //BMEditUtility.SaveDictionary(historyBuildInfoPath, historyBuildInfoPath);
            EditorPrefs.SetString(settings.AppName + BM_Build_Version, version.ToString());
            Logger.Log(string.Format("Bundle total num:{0}", total));
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
                    string temp = path + "." + BMUtility.EncryptWithMD5(path) + ".bytes";
                    if (File.Exists(temp))
                        File.Delete(temp);
                    if (isLuaEncoded)
                    {
                        if (!EncodeLuaFile(path, temp))
                        {
                            Debug.Log("Encode lua file fail!");
                            break;
                        }
                    }
                    else
                    {
                        File.Copy(path, temp);
                    }
                    buildInfo.assetPaths.Add(temp);
                    tempLuaPaths.Add(temp);
                }
                else
                {
                    buildInfo.assetPaths.Add(path);
                }
                //Logger.Log("path:{0}", path);
                EditorUtility.DisplayCancelableProgressBar("Fetch Build Info...", path, (float)(i + 1.0f) / (float)resFiles.Length);
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
                List<string> list = new List<string>();
                for (int i = 0; i < dependencePaths.Length; i++)
                {
                    if(!dependencePaths[i].EndsWith(".cs"))
                        list.Add(dependencePaths[i].ToLower());
                }
                //移除本身
                if(list.Count > 0)
                    list.Remove(filePath.ToLower());
                if(list.Count > 0)
                    dependenceMap[filePath] = list.ToArray();
            }
        }
        
        //加密lua
        static bool EncodeLuaFile(string srcFile, string outFile)
        {
            string AppDataPath = Application.dataPath.Replace("Assets", "");
            string luaexe   = string.Empty;
            string exedir   = string.Empty;
            bool   isWin    = true;
            string args     = string.Format("-b -g {0} {1}",AppDataPath + srcFile, AppDataPath + outFile);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                isWin   = true;
                luaexe  = LuaUtils.isX64 ? "luajit64.exe" : "luajit32.exe";
                exedir  = Path.Combine(AppDataPath, LuaUtils.isX64 ? "Tools/Luajit64/" : "Tools/Luajit/");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                isWin   = false;
                if (buildTarget == BuildTarget.StandaloneOSX)
                {
                    args = string.Format("-o {0} {1}",AppDataPath + outFile, AppDataPath + srcFile);
                    luaexe = "./luac";
                    exedir  = AppDataPath + "Tools/luavm/";
                }
                else
                {
                    luaexe = "./luajit";
                    exedir  = AppDataPath + "Tools/luajit_mac/";
                }
            }
            if (!Directory.Exists(exedir))
                throw new Exception(string.Format("Can not found Lua dir {0}",exedir));
            if (!File.Exists(srcFile))
                throw new Exception(string.Format("Can not found Source Lua file {0}",srcFile));
            if (!File.Exists(exedir + luaexe))
                throw new Exception(string.Format("Can not found Luajit.exe {0}{1}",exedir,luaexe));
            return LuaUtils.EncodeLuaFile(args, isWin,exedir, luaexe);
        }
    }
}
    