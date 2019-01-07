using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/**
 * EventDispatcher 类是可调度事件的所有类的基类
**/
public class EventTrigger
{
    //定义委托
    public delegate void EventDelegate(EventObj evt);

    protected Dictionary<String, List<EventDelegate>> captureListeners = null;

    protected Dictionary<String, List<EventDelegate>> bubbleListeners = null;

    protected EventTrigger _parent;

    public void addEventListener(String types, EventDelegate listener, bool useCapture = false, int priority = 0, bool useWeakReference = false)
    {
        Dictionary<String, List<EventDelegate>> listeners = null;

        if (listener == null)
        {
            throw new ArgumentNullException("Parameter listener must be non-null.");
        }
        if (useCapture)
        {
            if (captureListeners == null) captureListeners = new Dictionary<string, List<EventDelegate>>();
            listeners = captureListeners;
        }
        else
        {
            if (bubbleListeners == null) bubbleListeners = new Dictionary<string, List<EventDelegate>>();
            listeners = bubbleListeners;
        }
        List<EventDelegate> vector = null;
        if (listeners.ContainsKey(types))
        {
            vector = listeners[types];
        }
        if (vector == null)
        {
            vector = new List<EventDelegate>();
            listeners.Add(types, vector);
        }
        if (vector.IndexOf(listener) < 0)
        {
            vector.Add(listener);
        }
    }

    public void removeEventListener(String types, EventDelegate listener, bool useCapture = false)
    {
        if (listener == null)
        {
            throw new ArgumentNullException("Parameter listener must be non-null.");
        }
        Dictionary<string, List<EventDelegate>> listeners = useCapture ? captureListeners : bubbleListeners;
        if (listeners != null)
        {
            List<EventDelegate> vector = null;
            listeners.TryGetValue(types, out vector);
            if (vector != null)
            {
                int i = vector.IndexOf(listener);
                if (i >= 0)
                {
                    vector.Remove(listener);

                    if (vector.Count == 0)
                        listeners.Remove(types);

                    foreach (KeyValuePair<String, List<EventDelegate>> o in listeners)
                    {
                        if (o.Key == null)
                        {
                            if (listeners == captureListeners)
                            {
                                captureListeners = null;
                            }
                            else
                            {
                                bubbleListeners = null;
                            }
                        }
                    }
                }
            }
        }

    }

    public bool hasEventListener(String types)
    {
        return (captureListeners != null && captureListeners.ContainsKey(types)) || (bubbleListeners != null && bubbleListeners.ContainsKey(types));
    }

    public bool willTrigger(String types)
    {
        for (EventTrigger _object = this; _object != null; _object = _object._parent)
        {
            if ((_object.captureListeners != null && _object.captureListeners.ContainsKey(types)) || (_object.bubbleListeners != null && _object.bubbleListeners.ContainsKey(types)))
                return true;
        }
        return false;
    }

    public bool dispatchEvent(EventObj evt)
    {
        if (evt == null)
        {
            throw new ArgumentNullException("Parameter EventObject must be non-null.");
        }
        EventObj event3D = evt;
        if (event3D != null)
        {
            event3D.setTarget = this;
        }
        List<EventTrigger> branch = new List<EventTrigger>();
        int branchLength = 0;
        EventTrigger _object;
        int i, j = 0;
        int length;
        List<EventDelegate> vector;
        List<EventDelegate> functions;
        for (_object = this; _object != null; _object = _object._parent)
        {
            branch.Add(_object);
            branchLength++;
        }
        for (i = branchLength - 1; i > 0; i--)
        {
            _object = branch[i];
            if (event3D != null)
            {
                event3D.setCurrentTarget = _object;
                event3D.setEventPhase = EventPhase.CAPTURING_PHASE;
            }
            if (_object.captureListeners != null)
            {
                _object.captureListeners.TryGetValue(evt.types, out vector);
                if (vector != null)
                {
                    length = vector.Count;
                    functions = new List<EventDelegate>();
                    for (j = 0; j < length; j++) functions[j] = vector[j];
                    for (j = 0; j < length; j++) (functions[j] as EventDelegate)(evt);
                }
            }
        }
        if (event3D != null)
        {
            event3D.setEventPhase = EventPhase.AT_TARGET;
        }
        for (i = 0; i < branchLength; i++)
        {
            _object = branch[i];
            if (event3D != null)
            {
                event3D.setCurrentTarget = _object;
                if (i > 0)
                {
                    event3D.setEventPhase = EventPhase.BUBBLING_PHASE;
                }
            }
            if (_object.bubbleListeners != null)
            {
                _object.bubbleListeners.TryGetValue(evt.types, out vector);
                if (vector != null)
                {
                    length = vector.Count;
                    functions = new List<EventDelegate>();
                    for (j = 0; j < length; j++) functions.Add(vector[j]);
                    for (j = 0; j < length; j++) (functions[j] as EventDelegate)(evt);
                }
            }
            if (!event3D.bubbles) break;
        }
        return true;
    }

}