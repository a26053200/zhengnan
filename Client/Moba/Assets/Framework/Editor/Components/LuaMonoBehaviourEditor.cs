using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Framework;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/7/4 23:11:53</para>
/// </summary> 

[CustomEditor(typeof(LuaMonoBehaviour))]
public class LuaMonoBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LuaMonoBehaviour lb = (LuaMonoBehaviour)target;

        EditorGUILayout.LabelField("Behaviours:");
        EditorGUI.indentLevel++;
        foreach (var funName in lb.behaviourFun.Keys)
        {
            EditorGUILayout.LabelField(funName);
        }
        EditorGUI.indentLevel--;
    }
}

