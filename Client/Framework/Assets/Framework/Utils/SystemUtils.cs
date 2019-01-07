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
}

