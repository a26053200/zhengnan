using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
/// <summary>
/// <para>黄金分割工具</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/1/24 0:11:44</para>
/// </summary> 
public static class GoldenSection
{
    //高度固定 设置宽度为黄金分割点
    [MenuItem("Tools/Golden Section/Width Section")]
    static void GoldenSectionWidth()
    {
        if (Selection.activeObject)
        {
            GameObject selObj = Selection.activeObject as GameObject;
            RectTransform rect = selObj.GetComponent<RectTransform>();
            if(rect)
                rect.sizeDelta = new Vector2(Mathf.Floor(rect.sizeDelta.y / 0.618f), rect.sizeDelta.y);
        }
    }

    //宽度固定 设置高度为黄金分割点
    [MenuItem("Tools/Golden Section/Height Section")]
    static void GoldenSectionHeight()
    {
        if (Selection.activeObject)
        {
            GameObject selObj = Selection.activeObject as GameObject;
            RectTransform rect = selObj.GetComponent<RectTransform>();
            if (rect)
                rect.sizeDelta = new Vector2(rect.sizeDelta.x,Mathf.Floor(rect.sizeDelta.x * 0.618f));
        }
    }
}

