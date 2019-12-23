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
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Tick Num",_ticker.luaHandlerList.list.Count.ToString());
            EditorGUILayout.LabelField("Pool Num",_ticker.tickPool.Count.ToString());
        }
    }
}