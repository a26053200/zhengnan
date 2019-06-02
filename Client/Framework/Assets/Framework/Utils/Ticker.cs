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


        private Dictionary<string, TickHandler> luaHandlerList = new Dictionary<string, TickHandler>();

        private List<string> keyList = new List<string>();
        private List<string> delList = new List<string>();

        private void Update()
        {
            //非要采用for的方法也可

            keyList.Clear();
            delList.Clear();
            keyList.AddRange(luaHandlerList.Keys);

            for (int i = 0; i < keyList.Count; i++)
            {
                var handler = luaHandlerList[keyList[i]];
                if (Time.time - handler.startTime > handler.delay)
                {
                    handler.luaFunc.BeginPCall();
                    handler.luaFunc.PCall();
                    handler.luaFunc.EndPCall();
                    delList.Add(keyList[i]);
                }
            }
            for (int i = 0; i < delList.Count; i++)
            {
                luaHandlerList.Remove(delList[i]);
            }
        }
        public bool Contain(string key)
        {
            return luaHandlerList.ContainsKey(key);
        }
        public void DelayCallback(string key, float delay, LuaFunction luaFunc, GameObject caller = null)
        {
            TickHandler handler = new TickHandler
            {
                key = key,
                luaFunc = luaFunc,
                startTime = Time.time,
                delay = delay,
            };
            luaHandlerList.Add(key, handler);
        }

        public void CancelDelayCallback(string key)
        {
            if (luaHandlerList.ContainsKey(key))
            {
                luaHandlerList.Remove(key);
            }
        }
    
    }
}

