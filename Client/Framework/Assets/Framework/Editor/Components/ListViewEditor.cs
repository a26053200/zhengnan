using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// <para></para>
    /// <para>Author: zhengnan </para>
    /// <para>Create: DATE TIME</para>
    /// </summary>
    [CustomEditor(typeof(ListView))]
    public class ListViewEditor : Editor
    {
        private SerializedProperty space_x;
        private SerializedProperty space_y;
        private ListView list;
        private void OnEnable()
        {
            space_x = serializedObject.FindProperty("space_x");
            space_y = serializedObject.FindProperty("space_y");
            list = target as ListView;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var oldValue = space_x.floatValue;
            EditorGUILayout.PropertyField(space_x, new GUIContent("Horizontal Space"));
            if (Math.Abs(oldValue - space_x.floatValue) > 0f)
                UpdateList();
            oldValue = space_y.floatValue;
            EditorGUILayout.PropertyField(space_y, new GUIContent("Vertical Space"));
            if (Math.Abs(oldValue - space_y.floatValue) > 0f)
                UpdateList();
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateList()
        {
            if(Application.isPlaying)
                list.RefreshData();
        }
    }
}