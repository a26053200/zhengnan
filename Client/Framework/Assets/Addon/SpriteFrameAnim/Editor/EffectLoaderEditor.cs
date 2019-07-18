using UnityEditor;

namespace SFA
{
    [CustomEditor(typeof(EffectLoader))]
    public class EffectLoaderEditor : Editor
    {
        EffectLoader effectLoader;

        void OnEnable()
        {
            effectLoader = target as EffectLoader;
            effectLoader.Init();
            if(effectLoader.effectObj)
            {
                SpriteAnim spriteAnim = effectLoader.effectObj.GetComponent<SpriteAnim>();
                spriteAnim.Awake();
            }
        }

        private void OnDisable()
        {
            //if (effectLoader.effectObj)
            //    DestroyImmediate(effectLoader.effectObj);
            //effectLoader.effectObj = null;
        }
    }
}
