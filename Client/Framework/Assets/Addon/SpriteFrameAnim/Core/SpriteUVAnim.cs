using System.Collections.Generic;
using UnityEngine;

namespace SFA
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SpriteUVAnim : MonoBehaviour
    {
        public Vector4[] spriteRects;

        MeshRenderer meshRenderer;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            Material mat = meshRenderer.sharedMaterial;
            mat.SetVectorArray("_Rects", spriteRects);
            //spriteRects = mat.GetVectorArray("_Rects");
            //for (int i = 0; i < spriteRects.Length; i++)
            //{
            //    Debug.Log(spriteRects[i].ToString());
            //}
        }

        //private Vector4[] toVector4()
        //{
        //    Vector4[] v4s = new Vector4[spriteRects.Length];
        //    for (int i = 0; i < spriteRects.Length; i++)
        //    {
        //        v4s[i] = new Vector4(spriteRects[i].x, spriteRects[i].y, spriteRects[i].width, spriteRects[i].height);
        //    }
        //    return v4s;
        //}
    }
}
