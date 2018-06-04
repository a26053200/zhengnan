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
    public static int Second2Millisecond(float second)
    {
        return Mathf.FloorToInt(second * 1000f);
    }
}

