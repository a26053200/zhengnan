using UnityEngine;
using UnityEditor;

namespace FastBehavior
{
    [CustomEditor(typeof(StateMachineManager))]
    public class FastLuaBehaviorEditor : BaseBehaviorEditor
    {
        private void OnEnable()
        {
            mgr = target as StateMachineManager;
            EditorApplication.update += OnEditUpdate;
        }

        private void DisplayPool<T>(string title, ObjectPool<T> pool) where T : PoolObject
        {
            LabelField(title,
                pool.ObjList.list.Count,
                pool.ObjPool.Count,
                pool.maxNum,
                pool.counter);
        }
        public override void OnInspectorGUI()
        {
            DisplayPool("Fast Lua Behavior", mgr.FastLuaBehaviorPool);
            DisplayPool("State Machine", mgr.StateMachinePool);
            DisplayPool("State Action ", mgr.StateActionPool);
            DisplayPool("State Node", mgr.StateNodePool);
        }
        void OnEditUpdate()
        {
            Repaint();
        }
        void OnDestroy()
        {
            EditorApplication.update -= OnEditUpdate;
        }
        void OnDisable()
        {
            EditorApplication.update -= OnEditUpdate;
        }
    }
}
