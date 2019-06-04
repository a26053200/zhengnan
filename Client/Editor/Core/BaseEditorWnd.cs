using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/6/3 22:59:48</para>
/// </summary> 
public class BaseEditorWnd : EditorWindow
{
    protected void TabBar(string[] labels)
    {
        EditorGUI.BeginChangeCheck();
    }
}

