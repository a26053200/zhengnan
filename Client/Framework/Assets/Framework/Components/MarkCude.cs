using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/5/22 21:58:00</para>
/// </summary> 
/// 

namespace Framework
{
    public class MarkCude : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color color = Color.yellow;
        [Range(0.01f, 100f)]
        private BoxCollider collider;
#endif
        private void Awake()
        {
#if UNITY_EDITOR
            collider = gameObject.GetComponent<BoxCollider>();
#else
        Destroy(this)
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (collider)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(transform.position + collider.center, collider.size);
            }
        }
#endif
    }
}
    

