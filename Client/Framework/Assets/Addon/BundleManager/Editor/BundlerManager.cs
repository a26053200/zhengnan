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

        [MenuItem("Tools/Build/Force Build Bundle(IOS)",false,7)]
        public static void ForceBuildIOS()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.iOS);
        }

        [MenuItem("Tools/Build/Force Build Bundle(Android)",false,7)]
        public static void ForceBuildAndroid()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.Android);
        }

        [MenuItem("Tools/Build/Force Build Bundle(Win64)",false,7)]
        public static void ForceBuildWin64()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("Tools/Build/Force Build Bundle(OSX)",false,7)]
        public static void ForceBuildOSX()
        {
            StartBuild(true, Language.zh_CN, BuildTarget.StandaloneOSX, true);
        }

        [MenuItem("Tools/Build/Build Bundle(IOS)",false,7)]
        public static void BuildIOS()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.iOS);
        }

        [MenuItem("Tools/Build/Build Bundle(Android)",false,7)]
        public static void BuildAndroid()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.Android);
        }

        [MenuItem("Tools/Build/Build Bundle(Win64)",false,7)]
        public static void BuildWin64()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("Tools/Build/Build Bundle(OSX)",false,7)]
        public static void BuildOSX()
        {
            StartBuild(false, Language.zh_CN, BuildTarget.StandaloneOSX, true);
        }

        [MenuItem("Tools/Build/Build Bundle_Test",false,7)]
        public static void Test()
        {
            StartBuild(isForceBuild, Language.zh_CN, BuildTarget.StandaloneWindows64, true);
        }

        //配置路径
        const string BMSettings_Path = "Assets/Res/BMSettings.asset";

        static bool isForceBuild = false;

        static Dictionary<string, string> argDict;

        static Dictionary<string, BuildSampleInfo> buildInfos;

        private static void StartBuild(bool isForceBuild, Language language, BuildTarget buildTarget, bool moveBundle = false)
        {
            //加载打包配置
            BundleBuilder.settings = AssetDatabase.LoadAssetAtPath<BMSettings>(BMSettings_Path);
            //Output_Path = Application.dataPath.Replace("Assets", "TestBundle");
            BundleBuilder.Output_Root_Path = Application.dataPath.Replace("Assets", BundleBuilder.settings.BuildOutoutDirName);
            BundleBuilder.Output_Path = BundleBuilder.Output_Root_Path + "/" + language.ToString() + "/" + buildTarget.ToString();
            string historyBuildInfoPath = BundleBuilder.Output_Path + "/" + BMConfig.BundlDataFile;
            BundleBuilder.historyBuildInfo = isForceBuild ? null : LoadHistoryBundleData(historyBuildInfoPath);
            BundleBuilder.StartBuild(isForceBuild, Language.zh_CN, buildTarget, true, moveBundle);
        }

        public static Dictionary<string, BuildSampleInfo> LoadHistoryBundleData(string bundlDataFilePath)
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

        [MenuItem("Tools/Build/BuildAssetBundle")]
        static void BuildAssetBundle()
        {
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //BuildTarget buildTarget = GetSelectedBuildTarget(GetCommandLineArgs("-buildTarget"));
            //Language language = (Language)Enum.Parse(typeof(Language), GetCommandLineArgs("-language"));
            //Debug.Log(string.Format("Start build asset bundle with -buildTarget:{0}  -language{1}", 123123123, 123123123));
            //StartBuild(true, language, buildTarget, true);
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //string s = null;
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA" + s.Length);
            BMEditUtility.SaveUTF8TextFile(@"D:\work\WorkSpace_Unity\mrpg_trunk\Build\test.text", "1.1.1.1.1");
        }
    }
}