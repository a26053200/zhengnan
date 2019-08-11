using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using LitJson;

namespace BM
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/8/8 0:26:40</para>
    /// </summary> 
    public static class BundlerManager
    {
        [MenuItem("Build/Force Build")]
        public static void ForceBuild()
        {
            isForceBuild = !isForceBuild;
        }
        [MenuItem("Build/Force Build")]
        public static void ForceBuild()
        {
            isForceBuild = !isForceBuild;
        }
        [MenuItem("Build/Force Build", true)]
        public static bool IsForceBuild()
        {
            return isForceBuild;
        }

        [MenuItem("Build/Build Bundle(IOS)")]
        public static void BuildIOS()
        {
            StartBuild(isForceBuild, Language.zh_CN, BuildTarget.iOS, false);
        }

        [MenuItem("Build/Build Bundle(Android)")]
        public static void BuildAndroid()
        {
            StartBuild(isForceBuild, Language.zh_CN, BuildTarget.Android, false);
        }

        [MenuItem("Build/Build Bundle(Win64)")]
        public static void BuildWin64()
        {
            StartBuild(isForceBuild, Language.zh_CN, BuildTarget.StandaloneWindows64, false);
        }

        [MenuItem("Build/Build Bundle_Test")]
        public static void Test()
        {
            StartBuild(isForceBuild, Language.zh_CN, BuildTarget.StandaloneWindows64, false);
        }

        //配置路径
        const string BMSettings_Path = "Assets/Res/BMSettings.asset";

        static bool isForceBuild = false;

        static Dictionary<string, string> argDict;

        static Dictionary<string, BuildSampleInfo> buildInfos;

        private static void StartBuild(bool isForceBuild, Language language, BuildTarget buildTarget, bool generate)
        {
            //加载打包配置
            BundleBuilder.settings = AssetDatabase.LoadAssetAtPath<BMSettings>(BMSettings_Path);
            //Output_Path = Application.dataPath.Replace("Assets", "TestBundle");
            BundleBuilder.Output_Root_Path = Application.dataPath.Replace("Assets", BundleBuilder.settings.BuildOutoutDirName);
            BundleBuilder.Output_Path = BundleBuilder.Output_Root_Path + "/" + language.ToString() + "/" + buildTarget.ToString();
            string historyBuildInfoPath = BundleBuilder.Output_Path + "/" + BMConfig.BundlDataFile;
            BundleBuilder.historyBuildInfo = LoadBundleData(historyBuildInfoPath);
            BundleBuilder.StartBuild(isForceBuild, Language.zh_CN, BuildTarget.StandaloneWindows64, generate);
        }

        public static Dictionary<string, BuildSampleInfo> LoadBundleData(string bundlDataFilePath)
        {
            string bundleData = BMUtility.LoadText(bundlDataFilePath);
            if (bundleData == null)
                return null;
            JsonData jsonData = JsonMapper.ToObject(bundleData);
            Debug.Log(bundleData);

            Dictionary<string, BuildSampleInfo> buildInfos = new Dictionary<string, BuildSampleInfo>();

            JsonData buildInfoJson = jsonData["bundles"];
            for (int i = 0; i < buildInfoJson.Count; i++)
            {
                BuildSampleInfo buildInfo = new BuildSampleInfo()
                {
                    bundleName = buildInfoJson[i]["bundleName"].ToString(),
                    assetPaths = BMUtility.JsonToArray(buildInfoJson[i], "assetPaths"),
                    assetHashs = BMUtility.JsonToArray(buildInfoJson[i], "assetHashs"),
                };
                buildInfos.Add(buildInfo.bundleName, buildInfo);
            }
            return buildInfos;
        }
        /// <summary>
        /// 根据命令行参数名称，获取对应的值
        /// </summary>
        /// <returns>The command line argument.</returns>
        /// <param name="argName">Argument name.</param>
        private static string GetCommandLineArgs(string argName)
        {
            if(argDict == null)
            {
                argDict = new Dictionary<string, string>();
                string[] args = Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length; i++)
                {
                    if (i < args.Length - 1)
                    {
                        string argValue = args[i + 1];
                        if (argValue.StartsWith("-", StringComparison.Ordinal)) return "";
                        argDict.Add(argName, argValue);
                    }
                }
            }
            if (argDict.TryGetValue(argName, out string arg))
            {
                return arg;
            }
            else
            {
                return "";
            }
        }
    }
}


