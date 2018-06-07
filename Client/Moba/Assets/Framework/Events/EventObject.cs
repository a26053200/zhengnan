using UnityEngine;
using System.Collections;
using System;
/**
 * EventObject 类作为创建 Event 对象的基类，当发生事件时，Event 对象将作为参数传递给事件侦听器。 
**/
public class EventObject
{
    private EventTrigger _target;
    private int _eventPhase;
    private EventTrigger _currentTarget;
    private bool _bubbles;
    private bool _cancelable;
    private String _types;

    public EventObject(String types, bool bubbles = false, bool cancelable = false)
    {
        this._types = types;
        this._bubbles = bubbles;
        this._cancelable = cancelable;
    }

    public EventTrigger target
    {
        get { return _target; }
    }

    internal EventTrigger setTarget
    {
        set { _target = value; }
    }

    public EventTrigger currentTarget
    {
        get { return _currentTarget; }
    }

    internal EventTrigger setCurrentTarget
    {
        set { _currentTarget = value; }
    }

    public int eventPhase
    {
        get { return _eventPhase; }
    }

    internal int setEventPhase
    {
        set { _eventPhase = value; }
    }

    public bool bubbles
    {
        get { return _bubbles; }
    }

    public String types
    {
        get { return _types; }
    }

    public bool cancelable
    {
        get { return _cancelable; }
    }
}