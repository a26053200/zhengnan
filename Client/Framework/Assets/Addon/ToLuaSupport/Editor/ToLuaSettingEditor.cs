using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ToLuaSupport
{
    [CustomEditor(typeof(ToLuaSetting))]
    public class ToLuaSettingEditor : Editor
    {
        private ToLuaSetting _setting;

        private Dictionary<string, bool> textAreaOpenFlagDict;

        private FieldInfo[] _fieldInfos;
        private void OnEnable()
        {
            _setting = target as ToLuaSetting;
            textAreaOpenFlagDict = new Dictionary<string, bool>();

            _fieldInfos = typeof(ToLuaSetting).GetFields();
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            foreach (var fieldInfo in _fieldInfos)
            {
                if (fieldInfo.Name.StartsWith("Lua"))
                {
                    LayoutTextArea(fieldInfo.Name);
                }
                else
                {
                    LayoutTextLine(fieldInfo.Name);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void LayoutTextLine(string propertyName)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(propertyName));
        }
        
        private void LayoutTextArea(string propertyName)
        {
            textAreaOpenFlagDict.TryGetValue(propertyName, out bool isOpen);
            isOpen = EditorGUILayout.Foldout(isOpen, propertyName);
            if (isOpen)
            {
                SerializedProperty serializedProperty = serializedObject.FindProperty(propertyName);
                string content = serializedProperty.stringValue;
                EditorGUILayout.LabelField(propertyName);
                content = EditorGUILayout.TextArea(content);
                serializedProperty.stringValue = content;
            }
            if(!textAreaOpenFlagDict.ContainsKey(propertyName))
                textAreaOpenFlagDict.Add(propertyName, isOpen);
            textAreaOpenFlagDict[propertyName] = isOpen;
        }
    }
}