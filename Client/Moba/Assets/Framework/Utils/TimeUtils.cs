using System;
using UnityEngine;
using System.Collections.Generic;
//
// 时间工具
// Author: zhengnan
// Create: 2018/6/5 0:16:10
// 
public class TimeUtils
{
    static readonly string TIME_FORMAT = "yyyy-MM-dd-HH:mm:ss";

    /// <summary>
    /// 返回系统当前格式化 yyyy-MM-dd-HH:mm:ss 的时间字符串
    /// </summary>
    /// <returns></returns>
    public static String NowString()
    {
        return DateTime.Now.ToString(TIME_FORMAT);
    }

    public static int Second2Millisecond(float second)
    {
        return Mathf.FloorToInt(second * 1000f);
    }
}

