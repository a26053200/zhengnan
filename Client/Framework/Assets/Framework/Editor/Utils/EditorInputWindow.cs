using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/8/13 22:36:26</para>
/// </summary> 
public class EditorInputWindow : EditorWindow
{
    public delegate void InputCommit(string text);
    public static EditorInputWindow show(InputCommit func)
    {
        Vector2 winSize = new Vector2(400,122);
        EditorInputWindow wnd = GetWindowWithRect<EditorInputWindow>(new Rect((Screen.width - winSize.x) * 0.5f, (Screen.height - winSize.y) * 0.5f, winSize.x, winSize.y));
        wnd.inputCommit = func;
        wnd.Show();
        return wnd;
    }
    public string text;

    private InputCommit inputCommit;

    private EditorKeyboardEvent keyboardEvent;

    public void Init()
    {
        keyboardEvent = new EditorKeyboardEvent();
    }
    public void OnEnable()
    {
        keyboardEvent = new EditorKeyboardEvent();
    }
    private void OnGUI()
    {
        EditorUtils.DrawHorizontalSplitter(30);
        //EditorGUILayout.Space();
        //EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", "输入:", GUILayout.Width(35));
        text = EditorGUILayout.TextField("", text);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("","", GUILayout.Width(155));
        if (GUILayout.Button("确定") || keyboardEvent.FunctionKeyCode(KeyCode.KeypadEnter))
        {
            if (inputCommit != null)
                inputCommit(text);
            this.Close();
        }
        EditorGUILayout.LabelField("", "", GUILayout.Width(155));
        EditorGUILayout.EndHorizontal();
    }
}

