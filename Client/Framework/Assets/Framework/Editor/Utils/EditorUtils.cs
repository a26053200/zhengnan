﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.Events;
//
// Class Introduce
// Author: zhengnan
// Create: 2018/5/27 1:55:31
// 
public class EditorUtils
{
    public static void DisplayProgressBar<T>(string title, T[] dataList, UnityAction<T> progressFun)
    {
        for (int i = 0; i < dataList.Length; i++)
        {
            progressFun(dataList[i]);
            EditorUtility.DisplayProgressBar(title, string.Format("{0}/{1}", i + 1, dataList.Length), (float)(i + 1) / (float)dataList.Length);
        }
        EditorUtility.ClearProgressBar();
    }
    public static void DisplayProgressBar<T>(string title, List<T> dataList, UnityAction<T> progressFun)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            progressFun(dataList[i]);
            EditorUtility.DisplayProgressBar(title, string.Format("{0}/{1}", i + 1, dataList.Count), (float)(i + 1) / (float)dataList.Count);
        }
        EditorUtility.ClearProgressBar();
    }

    //横向分割线
    public static void DrawHorizontalSplitter(float height = 5)
    {
        GUILayout.Box("",
        GUILayout.Height(height),
        GUILayout.MaxHeight(height),
        GUILayout.MinHeight(height),
        GUILayout.ExpandWidth(true));
    }
    //纵向向分割线
    public static void DrawVerticalSplitter(float width = 5)
    {
        GUILayout.Box("",
        GUILayout.Width(width),
        GUILayout.MaxWidth(width),
        GUILayout.MinWidth(width),
        GUILayout.ExpandHeight(true));
    }
}

