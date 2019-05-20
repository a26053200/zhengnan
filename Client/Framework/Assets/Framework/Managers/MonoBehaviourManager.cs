using System;
using UnityEngine;
using System.Collections.Generic;
using Framework;
using LuaInterface;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/7/4 22:01:42</para>
/// </summary> 

namespace Framework
{
    public class MonoBehaviourManager : BaseManager
    {
        private List<LuaFunction> updateList;
        private List<LuaFunction> lateUpdateList;
        private List<LuaFunction> fixedUpdateList;

        private void Awake()
        {
            updateList = new List<LuaFunction>();
            lateUpdateList = new List<LuaFunction>();
            fixedUpdateList = new List<LuaFunction>();
        }
        private void Update()
        {
            for (int i = 0; i < updateList.Count; i++)
                Call(updateList[i]);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < lateUpdateList.Count; i++)
                Call(lateUpdateList[i]);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < fixedUpdateList.Count; i++)
                Call(fixedUpdateList[i]);
        }

        public void AddUpdateFun(LuaFunction func)
        {
            if(!updateList.Contains(func))
                updateList.Add(func);
        }

        public void AddLateUpdateFun(LuaFunction func)
        {
            if (!lateUpdateList.Contains(func))
                lateUpdateList.Add(func);
        }

        public void AddFixedUpdateFun(LuaFunction func)
        {
            if (!fixedUpdateList.Contains(func))
                fixedUpdateList.Add(func);
        }

        public void RemoveUpdateFun(LuaFunction func)
        {
            if (updateList.Contains(func))
                updateList.Remove(func);
        }

        public void RemoveLateUpdateFun(LuaFunction func)
        {
            if (lateUpdateList.Contains(func))
                lateUpdateList.Remove(func);
        }

        public void RemoveFixedUpdateFun(LuaFunction func)
        {
            if (fixedUpdateList.Contains(func))
                fixedUpdateList.Remove(func);
        }

        private void Call(LuaFunction func)
        {
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
        }
    }
}