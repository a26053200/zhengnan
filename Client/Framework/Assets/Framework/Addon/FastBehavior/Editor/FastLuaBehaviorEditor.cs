using UnityEngine;
using UnityEditor;

namespace FastBehavior
{
    [CustomEditor(typeof(StateMachine))]
    public class FastLuaBehaviorEditor : BaseBehaviorEditor
    {
        private void OnEnable()
        {
            stateMachine = target as StateMachine;
        }
        public override void OnInspectorGUI()
        {
            if (stateMachine.hideFlags == HideFlags.HideInInspector)
                return;
            //EditorGUI.BeginDisabledGroup(false);
            //{
            //    EditorGUILayout.ObjectField("Root Obj", stateMachine.gameObject, typeof(GameObject), true);
            //    EditorGUILayout.ObjectField("Root Machine", stateMachine, typeof(StateMachine), true);
            //}
            //EditorGUI.EndDisabledGroup();

            for (int i = 0; i < stateMachine.state2DList.Count; i++)
            {
                StateAction stateAction = stateMachine.state2DList[i];
                bool activeAction = stateAction == stateMachine.currState;
                LabelField("Action", stateAction.node.name, activeAction);
                EditorGUI.indentLevel++;
                for (int j = 0; j < stateMachine.fastBehavior.subBehaviors.Count; j++)
                {
                    FastLuaBehavior subBehavior = stateMachine.fastBehavior.subBehaviors[j];
                    if (subBehavior.parentNode == stateAction.node)
                    {
                        LabelField("Sub Behavior", subBehavior.id.ToString(), activeAction && subBehavior.stateMachine.enabled);
                        EditorGUI.indentLevel++;
                        DisplayStateMachine(subBehavior.stateMachine);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            //DisplayStateMachine(stateMachine);

            
        }

        private void DisplayStateMachine(StateMachine machine)
        {
            for (int i = 0; i < machine.state2DList.Count; i++)
            {
                StateAction stateAction = machine.state2DList[i];
                LabelField("Action", stateAction.node.name, stateAction == machine.currState);
            }
        }
    }
}
