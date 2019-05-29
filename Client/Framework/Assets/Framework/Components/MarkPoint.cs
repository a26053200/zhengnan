using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/5/22 21:53:03</para>
/// </summary> 

namespace Framework
{
    public class MarkPoint : MonoBehaviour
    {
        public Color color = Color.yellow;
        [Range(0.01f, 100f)]
        public float radius = 1;
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
    

