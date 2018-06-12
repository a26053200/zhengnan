﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
//
// Class Introduce
// Author: zhengnan
// Create: 2018/6/8 21:05:08
// 
public class StringUtils
{
    public static string Trim(string src)
    {
        StringBuilder sb = new StringBuilder();
        char[] charArr = src.ToCharArray();
        for (int i = 0; i < charArr.Length; i++)
        {
            if (!string.IsNullOrEmpty(charArr[i].ToString()))
                sb.Append(charArr[i]);
            //else
                //Debug.Log(charArr[i]);
        }
        return sb.ToString();
    }

    public static string ThrowControlChar(string src)
    {
        StringBuilder sb = new StringBuilder();
        char[] charArr = src.ToCharArray();
        for (int i = 0; i < charArr.Length; i++)
        {
            if (!(charArr[i] == '\\'))
                sb.Append(charArr[i]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 添加转义字符
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string AddContrlChar(string src)
    {
        StringBuilder sb = new StringBuilder();
        char[] charArr = src.ToCharArray();
        for (int i = 0; i < charArr.Length; i++)
        {
            switch(charArr[i])
            {
                case '"':
                case '\\':
                    sb.Append('\\');
                    sb.Append(charArr[i]);
                    break;
                default:
                    sb.Append(charArr[i]);
                    break;
            }
        }
        return sb.ToString();

    }

    public static string Bytes2String(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i]);
        }
        return sb.ToString();
    }
}

