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
            public int id;
            public LuaFunction luaFunc;
            public float startTime;
            public float delay;
            public bool delete;
            public bool ignoreTimeScale;
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

        [NoToLua]
        public Iterator<TickHandler> luaHandlerList = new Iterator<TickHandler>();
        [NoToLua]
        public Queue<TickHandler> tickPool = new Queue<TickHandler>();
        
        private void Awake()
        {
            for (int i = 0; i < 10; i++)
            {
                tickPool.Enqueue(new TickHandler()
                {
                    id = 0,
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
                if (handler != null && !handler.delete && getCurrentTime(handler.ignoreTimeScale) - handler.startTime > handler.delay)
                {
                    handler.luaFunc.BeginPCall();
                    handler.luaFunc.PCall();
                    handler.luaFunc.EndPCall();
                    handler.delete = true;
                    luaHandlerList.Remove(handler);
                    if(!tickPool.Contains(handler))
                        tickPool.Enqueue(handler);
                }
            }
        }

        private float getCurrentTime(bool ignoreTimeScale)
        {
            float time;
            if (ignoreTimeScale)
                return Time.realtimeSinceStartup;
            else
                return Time.time;
            return time;
        }

        public bool Contain(int id)
        {
            for (int i = 0; i < luaHandlerList.list.Count; i++)
            {
                if (luaHandlerList.list[i].id == id)
                    return true;
            }
            return false;
        }
        public void DelayCallback(int id, float delay, LuaFunction luaFunc, bool ignoreTimeScale = false)
        {
            TickHandler handler = Pop();
            handler.id = id;
            handler.luaFunc = luaFunc;
            handler.startTime = getCurrentTime(ignoreTimeScale);
            handler.delay = delay;
            handler.delete = false;
            handler.ignoreTimeScale = ignoreTimeScale;
            luaHandlerList.Add(handler);
        }

        private TickHandler Pop()
        {
            if (tickPool.Count > 0)
            {
                return tickPool.Dequeue();
            }
            else
            {
                return new TickHandler()
                {
                    id = 0,
                    luaFunc = null,
                    startTime = 0,
                    delay = 0,
                    delete = false
                };
            }
        }

        public void CancelDelayCallback(int id)
        {
            for (int i = 0; i < luaHandlerList.list.Count; i++)
            {
                if (luaHandlerList.list[i].id == id)
                {
                    var temp = luaHandlerList.list[i];
                    luaHandlerList.Remove(temp);
                    temp.delete = true;
                    if(!tickPool.Contains(temp))
                        tickPool.Enqueue(temp);
                    break;
                }
            }
        }
    }
}

