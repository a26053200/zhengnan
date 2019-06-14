using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/5/22 22:05:15</para>
/// </summary> 

namespace Framework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    public class MarkBox : MonoBehaviour
    {
        public Color color = Color.yellow;
        private MeshFilter cubeMesh;

        private void Awake()
        {
#if UNITY_EDITOR
            
#else
        Destroy(this);
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            cubeMesh = gameObject.GetComponent<MeshFilter>();
            if(cubeMesh)
            {
                Gizmos.color = color;
                Gizmos.DrawWireMesh(cubeMesh.sharedMesh, transform.position, transform.rotation, transform.localScale);
            }
        }
#endif
    }
}
    

