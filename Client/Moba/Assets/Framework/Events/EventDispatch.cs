using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class EventDispatch
{
    private EventTrigger _eventDispatch;
    private Dictionary<String, EventTrigger.EventDelegate> _eventMap;
    public EventDispatch()
    {
        _eventDispatch = new EventTrigger();
    }
    public EventDispatch(EventTrigger eventDispatch)
    {
        _eventDispatch = eventDispatch;
    }
    public void dispatchEvent(EventObj evt)
    {
        _eventDispatch.dispatchEvent(evt);
    }
    public void addEventListener(String eventType, EventTrigger.EventDelegate eventObj)
    {
        if (_eventMap == null)
            _eventMap = new Dictionary<string, EventTrigger.EventDelegate>();
        if (_eventMap.ContainsKey(eventType))
        {
            throw new Exception("事件重复注册" + eventType);
        }
        else
        {
            _eventMap.Add(eventType, eventObj);
            _eventDispatch.addEventListener(eventType, eventObj);
        }
    }
    public void removeEeventListener(String eventType, EventTrigger.EventDelegate eventObj)
    {
        if (_eventMap == null)
            return;
        if (_eventMap.ContainsKey(eventType))
        {
            _eventDispatch.removeEventListener(eventType, eventObj);
            _eventMap.Remove(eventType);
        }
    }
    public void removeAllEvent()
    {
        if (_eventMap == null)
            return;
        foreach (var eventObj in _eventMap)
        {
            _eventDispatch.removeEventListener(eventObj.Key, eventObj.Value);
        }
        _eventMap.Clear();
        _eventMap = null;
    }
}

