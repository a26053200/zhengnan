using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>lua 引导启动</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/6/10 0:40:41</para>
/// </summary> 
public class LuaBootstrap : LuaClient
{
    protected override void OpenLibs()
    {
        base.OpenLibs();
        OpenCJson();//打开cjson
    }
}

