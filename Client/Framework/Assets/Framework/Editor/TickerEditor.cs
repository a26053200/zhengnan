using System;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(Ticker))]
    public class TickerEditor : Editor
    {
        private Ticker _ticker;
        private void OnEnable()
        {
            _ticker = target as Ticker;
            EditorApplication.update += OnEditUpdate;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Tick Num",_ticker.luaHandlerList.list.Count.ToString());
            EditorGUILayout.LabelField("Pool Num",_ticker.tickPool.Count.ToString());
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