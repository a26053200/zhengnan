using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
/// <summary>
/// Introduction: ScrollListEditor
/// Author: 	Cheng
/// Time: 
/// </summary>
[CustomEditor(typeof(ScrollList))]
public class ScrollListEditor : Editor
{
    ScrollList script;
    void OnEnable()
    {
        script = (ScrollList)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("排列方式");
        script.arrangement = (ScrollList.Arrangement)EditorGUILayout.EnumPopup(script.arrangement);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("对齐方式");
        EditorGUILayout.BeginVertical();
        script.verticalAlign = (ScrollList.VerticalAlign)EditorGUILayout.EnumPopup(script.verticalAlign);
        script.horizontalAlign = (ScrollList.HorizontalAlign)EditorGUILayout.EnumPopup(script.horizontalAlign);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("行距");
        script.rowSpace = EditorGUILayout.FloatField(script.rowSpace);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("列距");
        script.columuSpace = EditorGUILayout.FloatField(script.columuSpace);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        if (script.arrangement == ScrollList.Arrangement.Horizontal)
            EditorGUILayout.PrefixLabel("每列个数");
        else
            EditorGUILayout.PrefixLabel("每行个数");
        int num = EditorGUILayout.IntField(script.MaxPerLine);
        if (num > 0 && num != script.MaxPerLine)
            script.MaxPerLine = num;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("边缘距离");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("上");
        script.marginTop = EditorGUILayout.FloatField(script.marginTop);
        GUILayout.Label("下");
        script.marginBottom = EditorGUILayout.FloatField(script.marginBottom);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("左");
        script.marginLeft = EditorGUILayout.FloatField(script.marginLeft);
        GUILayout.Label("右");
        script.marginRight = EditorGUILayout.FloatField(script.marginRight);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("总个数");
        script.ChildCount = EditorGUILayout.IntField(script.ChildCount);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("显示窗口");
        script.ViewPort = EditorGUILayout.Vector2Field("", script.ViewPort);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("ItenRender");
        script.item = (GameObject)EditorGUILayout.ObjectField(script.item, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
            script.ReBuild();
    }
}


