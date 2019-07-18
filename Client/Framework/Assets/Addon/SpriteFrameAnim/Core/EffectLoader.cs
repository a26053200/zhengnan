using UnityEngine;
using System.Collections;

public class EffectLoader : MonoBehaviour
{
    public GameObject effectPrefab;

    [HideInInspector]
    public GameObject effectObj;

    // Use this for initialization
    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        if(!effectObj && effectPrefab)
        {
            effectObj = Instantiate(effectPrefab);
            if(!Application.isPlaying)
            {
                effectObj.hideFlags = HideFlags.DontSaveInEditor;
            }
            effectObj.transform.SetParent(transform);
            effectObj.transform.localPosition = Vector3.zero;
            effectObj.transform.localEulerAngles = Vector3.zero;
            effectObj.transform.localScale = Vector3.one;
        }
    }
}
