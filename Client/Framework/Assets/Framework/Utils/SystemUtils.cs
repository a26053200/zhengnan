using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// <para>系统工具</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/14 0:26:24</para>
/// </summary> 
public class SystemUtils
{
    //获取系统用户名
    public static string GetSystemUserName()
    {
        return Environment.UserName;
    }
    //获取运行堆栈
    public static string GetDebugStackTrace()
    {
        StringBuilder sb = new StringBuilder();
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
        System.Diagnostics.StackFrame[] sfs = st.GetFrames();
        for (int i = 2; i < sfs.Length; ++i)
        {
            System.Reflection.MethodBase mb = sfs[i].GetMethod();
            sb.AppendFormat("\t{0}:{1}() (at {2}.cs: line {3}))\r\n",mb.DeclaringType.FullName,mb.Name,mb.DeclaringType.FullName.Replace(".","/"),sfs[i].GetFileLineNumber());
        }
        return sb.ToString();
    }

    /// <summary>
    /// 获取保存到本地的预设字符串
    /// 该函数区分了本地不同项目路径
    /// </summary>
    /// <param name="key">预设的key</param>
    /// <returns></returns>
    public static string GetPlayerPrefsString(string key,string defaultValue = "")
    {
        string md5Key = StringUtils.EncryptWithMD5(Application.dataPath + key);
        return PlayerPrefs.GetString(md5Key, defaultValue);
    }

    public static void SavePlayerPrefsString(string key, string value)
    {
        string md5Key = StringUtils.EncryptWithMD5(Application.dataPath + key);
        PlayerPrefs.SetString(md5Key, value);
    }

    /// <summary>
    /// 获取保存到本地的预设整数值
    /// 该函数区分了本地不同项目路径
    /// </summary>
    /// <param name="key">预设的key</param>
    /// <returns></returns>
    public static int GetPlayerPrefsInt(string key,int defaultValue = 0)
    {
        string md5Key = StringUtils.EncryptWithMD5(Application.dataPath + key);
        return PlayerPrefs.GetInt(md5Key, defaultValue);
    }

    public static void SavePlayerPrefsInt(string key, int value)
    {
        string md5Key = StringUtils.EncryptWithMD5(Application.dataPath + key);
        PlayerPrefs.SetInt(md5Key, value);
    }
    /// <summary>
    /// 获取保存到本地的预设浮点值
    /// 该函数区分了本地不同项目路径
    /// </summary>
    /// <param name="key">预设的key</param>
    /// <returns></returns>
    public static float GetPlayerPrefsFloat(string key, float defaultValue = 0)
    {
        string md5Key = StringUtils.EncryptWithMD5(Application.dataPath + key);
        return PlayerPrefs.GetFloat(md5Key, defaultValue);
    }

    public static void SavePlayerPrefsFloat(string key, float value)
    {
        string md5Key = StringUtils.EncryptWithMD5(Application.dataPath + key);
        PlayerPrefs.SetFloat(md5Key, value);
    }
}

