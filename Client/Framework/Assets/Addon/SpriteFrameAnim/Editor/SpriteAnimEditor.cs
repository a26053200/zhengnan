using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace SFA
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/7/18 0:20:24</para>
    /// </summary> 
    [CustomEditor(typeof(SpriteAnim))]
    public class SpriteAnimEditor : Editor
    {
        SpriteAnim spriteAnim;

        private int currFrame;

        private int oldInt;

        private void OnEnable()
        {
            spriteAnim = target as SpriteAnim;
            spriteAnim.Awake();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            oldInt = currFrame;
            currFrame = EditorGUILayout.IntSlider("Frame:", currFrame, 0, spriteAnim.FrameCount - 1);
            if (oldInt != currFrame)
                spriteAnim.SetSprite(currFrame);
        }
    }
}


