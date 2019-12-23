using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace Framework
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/6/2 1:46:11</para>
    /// </summary> 
    public class Ticker : MonoBehaviour
    {
        public class TickHandler
        {
            public GameObject caller;
            public string key;
            public LuaFunction luaFunc;
            public float startTime;
            public float delay;
            public bool delete;
        }
        private static Ticker s_instance;

        public static Ticker Launch()
        {
            if (s_instance == null)
            {
                GameObject tickObj = new GameObject("[Ticker]");
                DontDestroyOnLoad(tickObj);
                s_instance = tickObj.AddComponent<Ticker>();
            }
            else
            {
                throw new Exception("Ticker will must be single instance!");
            }
            return s_instance;
        }

        public Iterator<TickHandler> luaHandlerList = new Iterator<TickHandler>();

        public Stack<TickHandler> tickPool = new Stack<TickHandler>();
        
        private void Awake()
        {
            for (int i = 0; i < 10; i++)
            {
                tickPool.Push(new TickHandler()
                {
                    key = null,
                    luaFunc = null,
                    startTime = 0,
                    delay = 0,
                    delete = false
                });
            }
        }

        private void Update()
        {
            luaHandlerList.Reset();
            while (luaHandlerList.MoveNext())
            {
                var handler = luaHandlerList.Current as TickHandler;
                if (handler != null && !handler.delete && Time.time - handler.startTime > handler.delay)
                {
                    handler.luaFunc.BeginPCall();
                    handler.luaFunc.PCall();
                    handler.luaFunc.EndPCall();
                    handler.delete = true;
                    luaHandlerList.Remove(handler);
                    tickPool.Push(handler);
                }
            }
        }
        public bool Contain(string key)
        {
            for (int i = 0; i < luaHandlerList.list.Count; i++)
            {
                if (luaHandlerList.list[i].key == key)
                    return true;
            }
            return false;
        }
        public void DelayCallback(string key, float delay, LuaFunction luaFunc, GameObject caller = null)
        {
            TickHandler handler = Pop();
            handler.key = key;
            handler.luaFunc = luaFunc;
            handler.startTime = Time.time;
            handler.delay = delay;
            handler.delete = false;
            luaHandlerList.Add(handler);
        }

        private TickHandler Pop()
        {
            if (tickPool.Count > 0)
            {
                return tickPool.Pop();
            }
            else
            {
                return new TickHandler()
                {
                    key = null,
                    luaFunc = null,
                    startTime = 0,
                    delay = 0,
                    delete = false
                };
            }
        }

        public void CancelDelayCallback(string key)
        {
            for (int i = 0; i < luaHandlerList.list.Count; i++)
            {
                if (luaHandlerList.list[i].key == key)
                {
                    luaHandlerList.Remove(luaHandlerList.list[i]);
                }
            }
        }
    }
}

