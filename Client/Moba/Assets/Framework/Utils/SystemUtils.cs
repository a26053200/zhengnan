using System;
using UnityEngine;
using System.Collections.Generic;
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
}

