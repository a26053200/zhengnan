using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
//
// Class Introduce
// Author: zhengnan
// Create: 2018/6/8 21:05:08
// 
public class StringUtils
{
    /// <summary>
    /// 替换字符串中所有出现的字符
    /// </summary>
    /// <param name="src"></param>
    /// <param name="oldStr"></param>
    /// <param name="newStr"></param>
    /// <returns></returns>
    public static string ReplaceAll(string src, string oldStr, string newStr)
    {
        int index = -1;
        do
        {
            index = src.IndexOf(oldStr);
            if(index != -1)
                src = src.Replace(oldStr, newStr);
        } while (index != -1);
        return src;
    }

    /// <summary>
    /// 首字母小写
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string FirstToLower(string src)
    {
        StringBuilder sb = new StringBuilder(src);
        string first = sb[0].ToString();
        sb[0] = first.ToLower().ToCharArray()[0];
        return sb.ToString();
    }

    /// <summary>
    /// 首字母小写
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string FirstToUpper(string src)
    {
        StringBuilder sb = new StringBuilder(src);
        string first = sb[0].ToString();
        sb[0] = first.ToUpper().ToCharArray()[0];
        return sb.ToString();
    }

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

    public static string EncryptWithMD5(string source)
    {
        byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

        }
        return strbul.ToString();
    }
}

