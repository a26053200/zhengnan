using System;
using UnityEngine;
using System.Collections.Generic;

namespace BM
{
    /// <summary>
    /// <para>Logger</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/12 23:01:40</para>
    /// </summary> 
    public static class Logger
    {
        public static void Log(string msg, params object[] args)
        {
            Debug.Log(string.Format(msg, args));
        }

        public static void Error(string msg, params object[] args)
        {
            Debug.LogError(string.Format(msg, args));
        }
    }
}
    

