using LuaInterface;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2019/3/20 22:16:06</para>
/// </summary> 

namespace Framework
{
    public class EventTriggerListener : MonoBehaviour, 
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,IPointerEnterHandler, IPointerExitHandler, 
        //IInitializePotentialDragHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler
        //IDropHandler,
        //IMoveHandler, 
        //IScrollHandler

    {
        
        #region 17个监听事件，一套带数据，一套不带数据
        public delegate void EventDelegate(BaseEventData eventData);
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

        public bool passEvent;
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
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.pointerEnterHandler);
            if (onEventEnter != null) onEventEnter(eventData);
            if (onEnter != null) onEnter(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.pointerExitHandler);
            if (onEventExit != null) onEventExit(eventData);
            if (onExit != null) onExit(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.pointerDownHandler);
            if (onEventDown != null) onEventDown(eventData);
            if (onDown != null) onDown(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.pointerUpHandler);
            if (onEventUp != null) onEventUp(eventData);
            if (onUp != null) onUp(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.pointerClickHandler);
            if (onEventClick != null) onEventClick(eventData);
            if (onClick != null) onClick(gameObject);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.initializePotentialDrag);
            if (onEventInitializeDrag != null) onEventInitializeDrag(eventData);
            if (onInitializeDrag != null) onInitializeDrag(gameObject);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.beginDragHandler);
            if (onEventBeginDrag != null) onEventBeginDrag(eventData);
            if (onBeginDrag != null) onBeginDrag(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.dragHandler);
            if (onEventDrag != null) onEventDrag(eventData);
            if (onDrag != null) onDrag(gameObject);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.endDragHandler);
            if (onEventEndDrag != null) onEventEndDrag(eventData);
            if (onEndDrag != null) onEndDrag(gameObject);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.dragHandler);
            if (onEventDrop != null) onEventDrop(eventData);
            if (onDrop != null) onDrop(gameObject);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (passEvent) EventHelper.PassEvent(eventData, ExecuteEvents.scrollHandler);
            if (onEventScroll != null) onEventScroll(eventData);
            if (onScroll != null) onScroll(gameObject);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (onEventMove != null) onEventMove(eventData);
            if (onMove != null) onMove(gameObject);
        }
        
        

    }

}

