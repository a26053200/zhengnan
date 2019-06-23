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
            tailLength = StringUtils.EncryptWithMD5("md5").Length + ".txt".Length + 1;

            LoadLuaScriptBundle(GlobalConsts.LuaRootDir);
            LoadLuaScriptBundle(GlobalConsts.ToLuaRootDir);
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

    protected void LoadLuaScriptBundle(string luaRootDir)
    {
        ResLoader resLoader = GameManager.GetResLoader();
        AssetBundle luaBundle = resLoader.GetBundleByBundleName(BMUtility.Path2Name(luaRootDir.ToLower()));
        LuaFileUtils.Instance.AddSearchBundle(luaRootDir.ToLower(), luaBundle);
        string[] allNames = luaBundle.GetAllAssetNames();
        for (int i = 0; i < allNames.Length; i++)
        {
            string name = allNames[i];
            name = name.Substring(0, name.Length - tailLength); //去掉md5后缀,转化为正确的Lua文件名
            name = name.Replace(luaRootDir.ToLower() + "/", "");
            name = "lua_" + BMUtility.Path2Name(name);
            Debug.LogFormat("{0} - {1}", name, allNames[i]);
            LuaFileUtils.Instance.AddLuaNameMap(name, allNames[i]);
        }
    }
}

