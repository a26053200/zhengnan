//
// Class Introduce
// Author: zhengnan
// Create: 2018/6/30 11:22:53
// 

using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Framework
{
    public static class LuaHelper
    {
        public static bool isNullObj(UnityEngine.Object obj)
        {
            return !obj;
        }

        public static void AddScrollListOnItemRender(ScrollList list, LuaFunction OnItemRender)
        {
            list.onItemRender = delegate (int index, Transform item)
            {
                OnItemRender.Call(index, item);
            };
        }

        public static void AddScrollListOnScrollOver(ScrollList list, LuaFunction onScrollOver)
        {
            list.onScrollOver = delegate (int index)
            {
                onScrollOver.Call(index);
            };
        }
        public static void AddButtonClick(GameObject go, LuaFunction func)
        {
            Button btn = go.GetComponent<Button>();
            if(btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(delegate()
                {
                    func.Call(go);
                });
            }
        }

        public static void RemoveButtonClick(GameObject go)
        {
            if(go)
            {
                Button btn = go.GetComponent<Button>();
                if (btn)
                    btn.onClick.RemoveAllListeners();
            }
        }

        public static Button[] GetChildrenButtons(GameObject go)
        {
            Button[] btns = go.GetComponentsInChildren<Button>();
            return btns;
        }

        public static LuaMonoBehaviour AddLuaMonoBehaviour(GameObject go, string componentName, string funName,LuaFunction func)
        {
            LuaMonoBehaviour luaMonoBehaviour = AddLuaComponent<LuaMonoBehaviour>(go, componentName);
            luaMonoBehaviour.SetBehaviour(funName, func);
            return luaMonoBehaviour;
        }

        static T AddLuaComponent<T>(GameObject go, string componentName) where T : LuaComponent
        {
            T[] olds = go.GetComponents<T>();
            for (int i = 0; i < olds.Length; i++)
            {
                if (olds[i].componentName == componentName)
                {
                    //Debug.LogError(string.Format("The LuaComponent name '{0}' has already added!", componentName));
                    return olds[i];
                }
            }
            T t = go.AddComponent<T>();
            t.componentName = componentName;
            return t;
        }

        /// <summary>
        /// 给GameObject 注册点击事件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="luaFunc"></param>
        /// <returns></returns>
        public static UnityEngine.EventSystems.EventTrigger AddObjectClickEvent(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.PointerClick, luaFunc);
        }
        /// <summary>
        /// 给GameObject 注册事件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="type"></param>
        /// <param name="luaFunc"></param>
        public static UnityEngine.EventSystems.EventTrigger AddObjectEvent(GameObject go, EventTriggerType type, LuaFunction luaFunc)
        {
            UnityEngine.EventSystems.EventTrigger trigger = go.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null)
                trigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = type;
            UnityEngine.Events.UnityAction<BaseEventData> eventHandler = new UnityEngine.Events.UnityAction<BaseEventData>(delegate(BaseEventData eventData)
            {
                luaFunc.BeginPCall();
                luaFunc.Call<BaseEventData> (eventData);
                luaFunc.EndPCall();
            });
            entry.callback.AddListener(eventHandler);
            trigger.triggers.Clear();
            trigger.triggers.Add(entry);
            return trigger;
        }
    }
}

