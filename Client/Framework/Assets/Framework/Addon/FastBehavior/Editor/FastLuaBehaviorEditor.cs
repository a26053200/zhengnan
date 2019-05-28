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
            //EditorGUI.BeginDisabledGroup(false);
            //{
            //    EditorGUILayout.ObjectField("Root Obj", stateMachine.gameObject, typeof(GameObject), true);
            //    EditorGUILayout.ObjectField("Root Machine", stateMachine, typeof(StateMachine), true);
            //}
            //EditorGUI.EndDisabledGroup();

            DisplayStateMachine(stateMachine);

            EditorGUI.indentLevel++;
            for (int i = 0; i < stateMachine.fastBehavior.subBehaviors.Count; i++)
            {
                FastLuaBehavior subBehavior = stateMachine.fastBehavior.subBehaviors[i];
                LabelField("Sub Behavior", subBehavior.id.ToString(), subBehavior.stateMachine.enabled);
                EditorGUI.indentLevel++;
                DisplayStateMachine(subBehavior.stateMachine);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
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
