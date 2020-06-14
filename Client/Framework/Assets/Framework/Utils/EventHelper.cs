using System.Collections.Generic;
using LuaInterface;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public static class EventHelper
    {
        public static void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            var current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                //判断穿透对象是否是需要要点击的对象
                if (current != results[i].gameObject)
                {
                    PointerEventData newData = new PointerEventData(EventSystem.current)
                    {
                        pointerPress = results[i].gameObject,
                        pointerDrag = results[i].gameObject,
                        pointerEnter = results[i].gameObject,
                    };
                    ExecuteEvents.Execute(results[i].gameObject, newData, function);
                }
            }
        }
        
        public static LuaFunction AddObjectEvent(GameObject gameObject, EventTriggerType type, LuaFunction luaFunc, bool passEvent)
        {
            EventTriggerListener.EventDelegate ed = delegate (BaseEventData eventData)
            {
                luaFunc.BeginPCall();
                luaFunc.Push(eventData);
                luaFunc.PCall();
                luaFunc.EndPCall();
            };
            EventTriggerListener listener = EventTriggerListener.Get(gameObject);
            listener.passEvent = passEvent;
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

        public static void ClearObjectEvent(GameObject gameObject)
        {
            EventTriggerListener listener = gameObject.GetComponent<EventTriggerListener>();
            if(listener)
                GameObject.Destroy(listener);
        }

        public static void RemoveObjectEvent(GameObject gameObject, LuaFunction luaFunc)
        {
            EventTriggerListener listener = gameObject.GetComponent<EventTriggerListener>();
            if(!listener)
                return;
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
                        case EventTriggerType.PointerDown:
                            listener.onEventDown -= ed;
                            break;
                        case EventTriggerType.PointerUp:
                            listener.onEventUp -= ed;
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
    }
}