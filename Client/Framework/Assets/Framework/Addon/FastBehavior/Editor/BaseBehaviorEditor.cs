using UnityEngine;
using UnityEditor;

namespace FastBehavior
{
    public class BaseBehaviorEditor : Editor
    {
        protected StateMachine stateMachine;

        protected void LabelField(string title, string content, bool enabled)
        {
            EditorGUI.BeginDisabledGroup(!enabled);
            {
                EditorGUILayout.LabelField(title, content);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
