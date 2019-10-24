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
    public static class BundleManager
    {
        [MenuItem("Tools/Build/Force Build Bundle(IOS)",false,1)]
        public static void ForceBuildIOS()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.iOS);
        }

        [MenuItem("Tools/Build/Force Build Bundle(Android)",false,1)]
        public static void ForceBuildAndroid()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.Android);
        }

        [MenuItem("Tools/Build/Force Build Bundle(Win64)",false,1)]
        public static void ForceBuildWin64()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("Tools/Build/Force Build Bundle(OSX)",false,1)]
        public static void ForceBuildOSX()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.StandaloneOSX);
        }

        [MenuItem("Tools/Build/Build Bundle(IOS)",false,2)]
        public static void BuildIOS()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.iOS);
        }

        [MenuItem("Tools/Build/Build Bundle(Android)",false,2)]
        public static void BuildAndroid()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.Android);
        }

        [MenuItem("Tools/Build/Build Bundle(Win64)",false,2)]
        public static void BuildWin64()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("Tools/Build/Build Bundle(OSX)",false,2)]
        public static void BuildOSX()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.StandaloneOSX);
        }

        //配置路径
        const string BMSettings_Path = "Assets/Res/BMSettings.asset";

        static Dictionary<string, string> argDict;

        private static void StartBuild(bool forceBuild, Language language, BuildTarget buildTarget)
        {
            //加载打包配置
            BundleBuilder.settings = AssetDatabase.LoadAssetAtPath<BMSettings>(BMSettings_Path);
            //Output_Path = Application.dataPath.Replace("Assets", "TestBundle");
            BundleBuilder.Output_Root_Path = Application.dataPath.Replace("Assets", BundleBuilder.settings.BuildOutoutDirName);
            BundleBuilder.Output_Path = BundleBuilder.Output_Root_Path + "/" + language.ToString() + "/" + buildTarget.ToString();
            string historyBuildInfoPath = BundleBuilder.Output_Path + "/" + BMConfig.BundleDataFile;
            BundleBuilder.historyBuildInfo = forceBuild ? null : LoadHistoryBundleData(historyBuildInfoPath);
            BundleBuilder.StartBuild(forceBuild, Language.zh_CN, buildTarget, true, false);
        }

        private static Dictionary<string, BuildSampleInfo> LoadHistoryBundleData(string bundlDataFilePath)
        {
            string bundleData = BMUtility.LoadText(bundlDataFilePath);
            if (bundleData == null)
                return null;
            JsonData jsonData = JsonMapper.ToObject(bundleData);
            Logger.Log("Load History Bundle Data:" + bundleData);

            Dictionary<string, BuildSampleInfo> buildInfos = new Dictionary<string, BuildSampleInfo>();

            JsonData buildInfoJson = jsonData["bundles"];
            for (int i = 0; i < buildInfoJson.Count; i++)
            {
                BuildSampleInfo buildInfo = new BuildSampleInfo()
                {
                    bundleName = buildInfoJson[i]["bundleName"].ToString(),
                    version = (int)buildInfoJson[i]["version"],
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

        static BuildTarget GetSelectedBuildTarget(string targetPlatform)
        {
            // 目标平台
            BuildTarget buildTarget = default(BuildTarget);
            switch (targetPlatform)
            {
                case "ios":
                    buildTarget = BuildTarget.iOS;
                    break;
                case "android":
                    buildTarget = BuildTarget.Android;
                    break;
                case "macos":
                    buildTarget = BuildTarget.StandaloneOSX;
                    break;
                case "windows":
                    buildTarget = BuildTarget.StandaloneWindows;
                    break;
                case "windows64":
                    buildTarget = BuildTarget.StandaloneWindows64;
                    break;
            }
            return buildTarget;
        }

        //[MenuItem("Tools/Build/BuildAssetBundle")]
        static void BuildAssetBundle()
        {
            Logger.Log("Start build asset bundle with command ");
            BuildTarget buildTarget = GetSelectedBuildTarget(GetCommandLineArgs("-buildTarget"));
            Language language = (Language)Enum.Parse(typeof(Language), GetCommandLineArgs("-language"));
            Debug.Log(string.Format("Start build asset bundle with -buildTarget:{0}  -language{1}", buildTarget, language));
            StartBuild(false, language, buildTarget);
        }
    }
}