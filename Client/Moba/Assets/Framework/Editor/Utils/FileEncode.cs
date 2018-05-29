using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
//
// Class Introduce
// Author: zhengnan
// Create: 2018/5/27 1:12:59
// 
public class FileEncode
{
    static string JavaPath = @"D:\work\MyWork\Java\NettyServer_old\src\";
    [MenuItem("Tools/File Encoding/java/UTF-8")]
    static void SetUTF_8()
    {
        SetFileEncode(JavaPath, new UTF8Encoding(false));
    }

    static void SetFileEncode(string path, Encoding encode)
    {
        string savePath = EditorUtility.OpenFolderPanel("转码后输出到", "", "");
        string[] filePaths = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories);
        EditorUtils.DisplayProgressBar<string>("File Encode Convert", filePaths,
            delegate(string filePath)
            {
                string dirPath = filePath.Replace(path, "");
                string destPath = Path.Combine(savePath, dirPath);
                byte[] content = FileUitls.GetFileData(filePath);
                Encoding srcEncode = GetBytesEncoding(content);
                byte[] res = Encoding.Convert(srcEncode, encode, content);
                FileUitls.SaveFileData(destPath, res);
            });
    }

    static Encoding GetBytesEncoding(byte[] bs)
    {
        int len = bs.Length;
        if (len >= 3 && bs[0] == 0xEF && bs[1] == 0xBB && bs[2] == 0xBF)
        {
            return Encoding.UTF8;
        }
        int[] cs = { 7, 5, 4, 3, 2, 1, 0, 6, 14, 30, 62, 126 };
        for (int i = 0; i < len; i++)
        {
            int bits = -1;
            for (int j = 0; j < 6; j++)
            {
                if (bs[i] >> cs[j] == cs[j + 6])
                {
                    bits = j;
                    break;
                }
            }
            if (bits == -1)
            {
                return Encoding.Default;
            }
            while (bits-- > 0)
            {
                i++;
                if (i == len || bs[i] >> 6 != 2)
                {
                    return Encoding.Default;
                }
            }
        }
        return Encoding.UTF8;
    }

    static string ReadAllFormatText(string filename)
    {
        byte[] bs = File.ReadAllBytes(filename);
        int len = bs.Length;
        if (len >= 3 && bs[0] == 0xEF && bs[1] == 0xBB && bs[2] == 0xBF)
        {
            return Encoding.UTF8.GetString(bs, 3, len - 3);
        }
        int[] cs = { 7, 5, 4, 3, 2, 1, 0, 6, 14, 30, 62, 126 };
        for (int i = 0; i < len; i++)
        {
            int bits = -1;
            for (int j = 0; j < 6; j++)
            {
                if (bs[i] >> cs[j] == cs[j + 6])
                {
                    bits = j;
                    break;
                }
            }
            if (bits == -1)
            {
                return Encoding.Default.GetString(bs);
            }
            while (bits-- > 0)
            {
                i++;
                if (i == len || bs[i] >> 6 != 2)
                {
                    return Encoding.Default.GetString(bs);
                }
            }
        }
        return Encoding.UTF8.GetString(bs);
    }
}

