using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace BM
{
    /// <summary>
    /// <para>BMWindow</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/13 22:13:32</para>
    /// </summary> 
    public class BMWindow : EditorWindow
    {
        [MenuItem("Window/Bundle Manager")]
        static void Init()
        {
            BMWindow window = (BMWindow)EditorWindow.GetWindow(typeof(BMWindow));
            window.ResetView();
            window.minSize = new Vector2(600, 400);
        }


        FieldInfo[] titleFields;
        BMSettings settings;

        void ResetView()
        {
            BMSettings.GetIntance().LoadSettings();
            settings = BMSettings.GetIntance();

            titleFields = settings.GetType().GetFields();
        }

        private void OnLostFocus()
        {
            Close();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings:");
            for (int i = 0; i < titleFields.Length; i++)
            {
                FieldInfo field = titleFields[i];
                if (field.FieldType == typeof(int))
                {
                    int oldValue = (int)field.GetValue(settings);
                    int value = EditorGUILayout.IntField(field.Name, oldValue);
                    if(oldValue != value)
                        field.SetValue(settings, value);
                }
                else if (field.FieldType == typeof(string))
                {
                    string oldValue = (string)field.GetValue(settings);
                    string value = EditorGUILayout.TextField(field.Name, oldValue);
                    if (oldValue != value)
                        field.SetValue(settings, value);
                }
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", "", GUILayout.Width(position.width * 0.618f));
            if (GUILayout.Button("Default"))
            {
                GUI.FocusControl(null);
                Repaint();
                settings.ResetToDefault();
            }
            if (GUILayout.Button("Save"))
            {
                settings.SaveSettings();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Build"))
            {
                BundleBuilder.Build();
            }
        }
    }
}
    

