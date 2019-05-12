using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/3/13 0:20:15</para>
/// </summary> 
public class PointerEventBinder : MonoBehaviour
{
    public object paramStr = "";

    public void OnClick(BaseEventData data)
    {
        Debug.Log("点击了cube tran=" + transform.name + ",paramStr=" + paramStr);
    }
}

