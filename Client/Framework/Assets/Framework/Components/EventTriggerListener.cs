using LuaInterface;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/3/20 22:16:06</para>
/// </summary> 

namespace Framework
{
    public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IInitializePotentialDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDropHandler,
    IMoveHandler, IScrollHandler

    {
        
        #region 17个监听事件，一套带数据，一套不带数据
        public delegate void EventDelegate(GameObject go, BaseEventData eventData);
        public delegate void VoidDelegate(GameObject go);

        public class Entry
        {
            public LuaFunction luaFunc { get; private set; }
            public EventDelegate eventDelegate { get; private set; }
            public VoidDelegate voidDelegate { get; private set; }
            public Entry(LuaFunction luaFunc, EventDelegate eventDelegate)
            {
                this.luaFunc = luaFunc;
                this.eventDelegate = eventDelegate;
            }
            public Entry(LuaFunction luaFunc, VoidDelegate voidDelegate)
            {
                this.luaFunc = luaFunc;
                this.voidDelegate = voidDelegate;
            }
        }

        public event EventDelegate onEventEnter, onEventExit, onEventDown, onEventUp, onEventClick, onEventInitializeDrag, onEventBeginDrag,
                                    onEventDrag, onEventMove, onEventEndDrag, onEventDrop, onEventScroll;
        public event VoidDelegate onEnter, onExit, onDown, onUp, onClick, onInitializeDrag, onBeginDrag, onDrag, onDrop, onEndDrag, onMove, onScroll;
        public Dictionary<EventTriggerType, Dictionary<LuaFunction, List<EventDelegate>>> luaFuncHash { get; private set; }
        #endregion
        public static EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null)
            {
                
                listener = go.AddComponent<EventTriggerListener>();
                listener.luaFuncHash = new Dictionary<EventTriggerType, Dictionary<LuaFunction, List<EventDelegate>>>();
            }
            
            return listener;
        }

        public List<EventDelegate> GetLuaFuncHashSet(EventTriggerType type, LuaFunction lunc)
        {
            Dictionary<LuaFunction, List<EventDelegate>> dict = null;
            if (!luaFuncHash.TryGetValue(type, out dict))
            {
                dict = new Dictionary<LuaFunction, List<EventDelegate>>();
                luaFuncHash.Add(type, dict);
            }
            List<EventDelegate> list = null;
            if (!dict.TryGetValue(lunc, out list))
            {
                list = new List<EventDelegate>();
                dict.Add(lunc, list);
            }
            return list;
        }

        public bool Contains(EventTriggerType type, LuaFunction lunc)
        {
            Dictionary<LuaFunction, List<EventDelegate>> dict = null;
            if (luaFuncHash.TryGetValue(type, out dict))
            {
                return dict.ContainsKey(lunc);
            }
            return false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onEventEnter != null) onEventEnter(gameObject, eventData);
            if (onEnter != null) onEnter(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (onEventExit != null) onEventExit(gameObject, eventData);
            if (onExit != null) onExit(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onEventDown != null) onEventDown(gameObject, eventData);
            if (onDown != null) onDown(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onEventUp != null) onEventUp(gameObject, eventData);
            if (onUp != null) onUp(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onEventClick != null) onEventClick(gameObject, eventData);
            if (onClick != null) onClick(gameObject);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (onEventInitializeDrag != null) onEventInitializeDrag(gameObject, eventData);
            if (onInitializeDrag != null) onInitializeDrag(gameObject);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onEventBeginDrag != null) onEventBeginDrag(gameObject, eventData);
            if (onBeginDrag != null) onBeginDrag(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onEventDrag != null) onEventDrag(gameObject, eventData);
            if (onDrag != null) onDrag(gameObject);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onEventEndDrag != null) onEventEndDrag(gameObject, eventData);
            if (onEndDrag != null) onEndDrag(gameObject);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (onEventDrop != null) onEventDrop(gameObject, eventData);
            if (onDrop != null) onDrop(gameObject);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (onEventScroll != null) onEventScroll(gameObject, eventData);
            if (onScroll != null) onScroll(gameObject);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (onEventMove != null) onEventMove(gameObject, eventData);
            if (onMove != null) onMove(gameObject);
        }

    }

}

