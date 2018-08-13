using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// <para>编辑器下的键盘事件</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/8/13 22:41:55</para>
/// </summary> 
public class EditorKeyboardEvent
{
    //public delegate void KeyUpAction();

    bool flagJudgeDownAllow = true;

    public bool FunctionKeyCode(KeyCode keyCode)
    {
        Event key = Event.current;
        bool flag = key.isKey && key.keyCode == keyCode;
        if (flag)//如果“事件”有效，并且“允许判断按下”。
        {
            if (key.type == EventType.KeyUp && !flagJudgeDownAllow)
            {
                //Debug.Log(key.keyCode.ToString() + " " + key.type.ToString() + " " + Time.time);
                flagJudgeDownAllow = true;//抬起按钮之后才允许继续判断
                return true;
            }
            else if (key.type == EventType.KeyDown && flagJudgeDownAllow)
            {
                flagJudgeDownAllow = false;//判断完了 就不允许判断了
                //Debug.Log(key.keyCode.ToString() + " " + key.type.ToString() + " " + Time.time);
            }
        }
        return false;
    }
}

