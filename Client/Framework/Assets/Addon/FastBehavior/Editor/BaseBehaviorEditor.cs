using UnityEngine;
using UnityEditor;

namespace FastBehavior
{
    public class BaseBehaviorEditor : Editor
    {
        //protected StateMachine stateMachine;
        
        protected StateMachineManager mgr;

        protected void LabelField(string title, int currNum,int poolNum,int maxNum,uint totalNum)
        {
            EditorGUILayout.LabelField(title);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Current Num",currNum.ToString());
            EditorGUILayout.LabelField("Pool Num",poolNum.ToString());
            EditorGUILayout.LabelField("Max Num",maxNum.ToString());
            EditorGUILayout.LabelField("Total Num",totalNum.ToString());
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}
