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

        static string Output_Path = @"D:\work\WorkSpace_Unity\mrpg_trunk\TestBundle";

        static string Any_File = "*.*";
        //配置
        static BMSettings settings;

        static List<BuildInfo> buildInfoList;


        //=======================
        // 流程函数
        //=======================
        public static List<BuildInfo> StartBuild()
        {
            //清空控制台日志
            Debug.ClearDeveloperConsole();
            //加载打包配置
            settings = AssetDatabase.LoadAssetAtPath<BMSettings>(BMSettings_Path);

            RemoveAllAssetBundleName();
            //获取所有Build信息
            FetchAllBuildInfo();

            GenAssetBundleInfos();

            GenAssetBundle();

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
            FetchBuildInfoList(settings.scenesFolderList, settings.scenesPattern, settings.scenesCompressType, true);
            FetchBuildInfoList(settings.completeFolderList, settings.completePattern, settings.completeCompressType, false, true);
        }

        static void FetchBuildInfoList(List<string> folders, string searchPattern, CompressType compressType, bool isScene = false, bool isCompleteAssets = false)
        {
            //包含所有资源的bundle
            for (int i = 0; i < folders.Count; i++)
            {
                if (!Directory.Exists(folders[i]))
                    continue;
                string resDir = BMEditUtility.Relativity2Absolute(folders[i]);
                BuildInfo buildInfo = FetchBuildInfo(resDir, searchPattern);
                buildInfo.isCompleteAssets = isCompleteAssets;
                buildInfo.isScene = isScene;
                buildInfo.compressType = compressType;
                buildInfoList.Add(buildInfo);
            }
        }

        static void GenAssetBundleInfos()
        {
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                buildInfo.assetBundleBuilds = new List<AssetBundleBuild>();
                if (buildInfo.isScene)
                {
                    
                }
                else
                {
                    for (int j = 0; j < buildInfo.assetPaths.Count; j++)
                    {
                        string path = buildInfo.assetPaths[j];
                        string name = path.Replace("\\", "_");
                        name = path.Replace("/", "_");
                        AssetBundleBuild assetBundleBuild = new AssetBundleBuild()
                        {
                            assetBundleName = name,
                            assetBundleVariant = settings.Suffix_Bundle,
                            assetNames = new string[] { path },
                            addressableNames = new string[] { path },
                        };
                        buildInfo.assetBundleBuilds.Add(assetBundleBuild);
                        EditorUtility.DisplayProgressBar("Gen AssetBundle Infos...", path, (float)(j + 1.0f) / (float)buildInfo.assetPaths.Count);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

        static void GenAssetBundle()
        {
            for (int i = 0; i < buildInfoList.Count; i++)
            {
                BuildInfo buildInfo = buildInfoList[i];
                if (buildInfo.isScene)
                {

                }
                else
                {
                    if(buildInfo.compressType == CompressType.LZ4)
                    {
                        BuildPipeline.BuildAssetBundles(Output_Path,
                            buildInfo.assetBundleBuilds.ToArray(),
                            BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression,
                            BuildTarget.StandaloneWindows64);
                    }else if (buildInfo.compressType == CompressType.LZMA)
                    {
                        BuildPipeline.BuildAssetBundles(Output_Path,
                            buildInfo.assetBundleBuilds.ToArray(),
                            BuildAssetBundleOptions.DeterministicAssetBundle,
                            BuildTarget.StandaloneWindows64);
                    }
                    else if (buildInfo.compressType == CompressType.None)
                    {
                        BuildPipeline.BuildAssetBundles(Output_Path,
                            buildInfo.assetBundleBuilds.ToArray(),
                            BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle,
                            BuildTarget.StandaloneWindows64);
                    }
                }
                EditorUtility.DisplayProgressBar("Gen AssetBundle...", (i + 1.0f) + "/" + buildInfoList.Count, (float)(i + 1.0f) / (float)buildInfoList.Count);
            }
            EditorUtility.ClearProgressBar();
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
                Logger.Log("path:{0}", path);
                EditorUtility.DisplayProgressBar("Fetch Build Info...", path, (float)(i + 1.0f) / (float)resFiles.Length);
            }
            EditorUtility.ClearProgressBar();
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
    