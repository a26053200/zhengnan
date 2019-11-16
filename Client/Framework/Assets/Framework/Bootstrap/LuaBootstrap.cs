using System.Collections.Generic;
using UnityEngine;
using Framework;
using LuaInterface;
using System.Text;
using BM;

/// <summary>
/// <para>lua 引导启动</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 0:40:41</para>
/// </summary> 
public class LuaBootstrap : LuaClient
{
    private int tailLength;

    protected override LuaFileUtils InitLoader()
    {
        if(GlobalConsts.isLuaBundleMode)
        {
            LuaFileUtils.Instance.beZip = true;
            // 主要为了获得md5串的长度
            // 例如 Assets/Lua/Main.lua.a19cffbd6db217c8a9f41869010f8a9e.txt
            tailLength = StringUtils.EncryptWithMD5("md5").Length + ".bytes".Length + 1;

            LoadLuaScriptBundle();
        }
        else
        {
            LuaFileUtils.Instance.beZip = false;
        }
        return base.InitLoader();
    }

    protected override void OpenLibs()
    {
        base.OpenLibs();
        

        OpenCJson();//打开cjson
    }

    protected void LoadLuaScriptBundle()
    {
        ResLoader resLoader = GameManager.GetResLoader();
        List<BundleInfo> bundleInfos = resLoader.bundleLoader.bundleInfos;
        for (int i = 0; i < bundleInfos.Count; i++)
        {
            if (bundleInfos[i].buildType == BuildType.Lua)
            {
                string bundleName = bundleInfos[i].bundleName;
                string luaRootDir = BMUtility.Name2Path(bundleName);
                AssetBundle luaBundle = resLoader.GetBundleByBundleName(bundleName);
                if (luaBundle)
                {
                    LuaFileUtils.Instance.AddSearchBundle(bundleName, luaBundle);
                    string[] allNames = luaBundle.GetAllAssetNames();
                    for (int j = 0; j < allNames.Length; j++)
                    {
                        string name = allNames[j];
                        name = name.Substring(0, name.Length - tailLength); //去掉md5后缀,转化为正确的Lua文件名
                        name = name.Replace(luaRootDir.ToLower() + "/", "");
                        name = "lua_" + BMUtility.Path2Name(name);
                        Debug.LogFormat("{0} - {1}", name, allNames[j]);
                        LuaFileUtils.Instance.AddLuaNameMap(name, allNames[j]);
                    }
                }
                else
                {
                    Logger.Error("There is not lua bundle:{0}",bundleName);
                }
            }
        }
    }
}

