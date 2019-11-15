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

        public static Button[] GetChildrenButtons(GameObject go, bool includeInactive = false)
        {
            Button[] btns = go.GetComponentsInChildren<Button>(includeInactive);
            return btns;
        }

        public static Component[] GetChildrenComponents(GameObject go, Type type, bool includeInactive = false)
        {
            Component[] coms = go.GetComponentsInChildren(type, includeInactive);
            return coms;
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
        public static LuaFunction AddObjectClickEvent(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.PointerClick, luaFunc);
        }
        public static LuaFunction AddObjectPointerEnter(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.PointerEnter, luaFunc);
        }
        public static LuaFunction AddObjectPointerExit(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.PointerExit, luaFunc);
        }
        public static LuaFunction AddObjectPointerDown(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.PointerDown, luaFunc);
        }
        public static LuaFunction AddObjectPointerUp(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.PointerUp, luaFunc);
        }
        public static LuaFunction AddObjectBeginDrag(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.BeginDrag, luaFunc);
        }
        public static LuaFunction AddObjectDrag(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.Drag, luaFunc);
        }
        public static LuaFunction AddObjectDrop(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.Drop, luaFunc);
        }
        public static LuaFunction AddObjectEndDrag(GameObject go, LuaFunction luaFunc)
        {
            return AddObjectEvent(go, EventTriggerType.EndDrag, luaFunc);
        }
        
        public static LuaFunction AddObjectEvent(GameObject gameObject, EventTriggerType type, LuaFunction luaFunc)
        {
            EventTriggerListener.EventDelegate ed = delegate (BaseEventData eventData)
            {
                luaFunc.BeginPCall();
                luaFunc.Push(eventData);
                luaFunc.PCall();
                luaFunc.EndPCall();
            };
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            //EventTriggerListener.Entry entry = new EventTriggerListener.Entry(luaFunc, ed);
            listener.GetLuaFuncHashSet(type, luaFunc).Add(ed);
            switch (type)
            {
                case EventTriggerType.PointerClick:
                    listener.onEventClick += ed;
                    break;
                case EventTriggerType.PointerEnter:
                    listener.onEventEnter += ed;
                    break;
                case EventTriggerType.PointerExit:
                    listener.onEventExit += ed;
                    break;
                case EventTriggerType.PointerDown:
                    listener.onEventDown += ed;
                    break;
                case EventTriggerType.PointerUp:
                    listener.onEventUp += ed;
                    break;
                case EventTriggerType.BeginDrag:
                    listener.onEventBeginDrag += ed;
                    break;
                case EventTriggerType.Drag:
                    listener.onEventDrag += ed;
                    break;
                case EventTriggerType.Drop:
                    listener.onEventDrop += ed;
                    break;
                case EventTriggerType.EndDrag:
                    listener.onEventEndDrag += ed;
                    break;
                default:
                    listener.GetLuaFuncHashSet(type, luaFunc).Remove(ed);
                    Logger.Error("No Register EventTriggerType:" + type.ToString());
                    break;
            }
            return luaFunc;
        }

        public static void RemoveObjectEvent(GameObject gameObject, LuaFunction luaFunc)
        {
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            foreach(var type in listener.luaFuncHash.Keys)
            {
                List<EventTriggerListener.EventDelegate> list = listener.GetLuaFuncHashSet(type, luaFunc);
                foreach (var ed in list)
                {
                    switch (type)
                    {
                        case EventTriggerType.PointerClick:
                            listener.onEventClick -= ed;
                            break;
                        case EventTriggerType.PointerEnter:
                            listener.onEventEnter -= ed;
                            break;
                        case EventTriggerType.PointerExit:
                            listener.onEventExit -= ed;
                            break;
                        case EventTriggerType.BeginDrag:
                            listener.onEventBeginDrag -= ed;
                            break;
                        case EventTriggerType.Drag:
                            listener.onEventDrag -= ed;
                            break;
                        case EventTriggerType.Drop:
                            listener.onEventDrop -= ed;
                            break;
                        case EventTriggerType.EndDrag:
                            listener.onEventEndDrag -= ed;
                            break;
                        default:
                            Logger.Error("No Register EventTriggerType:" + type.ToString());
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 给GameObject 注册事件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="type"></param>
        /// <param name="luaFunc"></param>
        //public static UnityEngine.EventSystems.EventTrigger.Entry AddObjectEvent(GameObject go, EventTriggerType type, LuaFunction luaFunc)
        //{
        //    UnityEngine.EventSystems.EventTrigger trigger = go.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        //    if (trigger == null)
        //        trigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        //    UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        //    entry.eventID = type;
        //    UnityEngine.Events.UnityAction<BaseEventData> eventHandler = new UnityEngine.Events.UnityAction<BaseEventData>(delegate(BaseEventData eventData)
        //    {
        //        luaFunc.BeginPCall();
        //        luaFunc.Call<BaseEventData> (eventData);
        //        luaFunc.EndPCall();
        //    });
        //    entry.callback.AddListener(eventHandler);
        //    //trigger.triggers.Clear();
        //    trigger.triggers.Add(entry);
        //    return entry;
        //}

//        public static void RemoveObjectEvent(GameObject go, UnityEngine.EventSystems.EventTrigger.Entry entry)
//        {
//            UnityEngine.EventSystems.EventTrigger trigger = go.GetComponent<UnityEngine.EventSystems.EventTrigger>();
//            if (trigger != null)
//            {
//                if (trigger.triggers.Contains(entry))
//                    trigger.triggers.Remove(entry);
//            }
//        }
        /// <summary>
        /// 判断是否按下(跨平台)
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerDown()
        {
            return Input.GetMouseButton(0) || (Input.touchCount > 0 && (int)Input.GetTouch(0).phase > -1);
        }
    }
}

