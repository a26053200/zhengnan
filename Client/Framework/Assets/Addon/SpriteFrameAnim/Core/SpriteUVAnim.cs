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
            
        }

        private void DebugVector4()
        {
            Material mat = meshRenderer.sharedMaterial;
            Vector4[] spriteRects = mat.GetVectorArray("_Rects");
            for (int i = 0; i < spriteRects.Length; i++)
            {
                Debug.Log(spriteRects[i].ToString());
            }
        }

        private void Update()
        {
            DebugVector4();
        }
    }
}
