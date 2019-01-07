using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
/// <summary>
/// <para>Class Introduce</para>
/// <para>Author: zhengnan</para>
/// <para>Create: 2018/7/4 22:04:46</para>
/// </summary> 

namespace Framework
{
    public class LuaMonoBehaviour : LuaComponent
    {
        public Dictionary<string, LuaFunction> behaviourFun = new Dictionary<string, LuaFunction>();

        private MonoBehaviourManager mgr
        {
            get { return GameManager.GetMonoBehaviourManager(); }
        }
        public void SetBehaviour(string funName, LuaFunction func)
        {
            behaviourFun[funName] = func;
            RegisterUpdate();
        }

        private void DoCall(string funName)
        {
            if (behaviourFun.ContainsKey(funName))
                behaviourFun[funName].Call();
        }

        private void Awake()
        {
            DoCall(BehaviourFunction.AWAKE);
        }

        private void Start()
        {
            DoCall(BehaviourFunction.START);
            RegisterUpdate();
        }

        private void OnEnable()
        {
            DoCall(BehaviourFunction.ON_ENABLE);
            RegisterUpdate();
        }

        private void OnDisable()
        {
            DoCall(BehaviourFunction.ON_DISABLE);
            RemoveUpdate();
        }

        private void OnDestroy()
        {
            if (behaviourFun.ContainsKey(BehaviourFunction.ON_DESTROY))
            {
                LuaFunction func = behaviourFun[BehaviourFunction.ON_DESTROY];
                if (func.GetLuaState() != null)
                    func.Call();
            }
            RemoveUpdate();
            if (behaviourFun != null)
                behaviourFun.Clear();
        }

        private void RegisterUpdate()
        {
            if (behaviourFun.ContainsKey(BehaviourFunction.UPDATE))
                mgr.AddUpdateFun(behaviourFun[BehaviourFunction.UPDATE]);
            if (behaviourFun.ContainsKey(BehaviourFunction.LATE_UPDATE))
                mgr.AddLateUpdateFun(behaviourFun[BehaviourFunction.LATE_UPDATE]);
            if (behaviourFun.ContainsKey(BehaviourFunction.FIXED_UPDATE))
                mgr.AddFixedUpdateFun(behaviourFun[BehaviourFunction.FIXED_UPDATE]);
        }

        private void RemoveUpdate()
        {
            if (behaviourFun.ContainsKey(BehaviourFunction.UPDATE))
                mgr.RemoveUpdateFun(behaviourFun[BehaviourFunction.UPDATE]);
            if (behaviourFun.ContainsKey(BehaviourFunction.LATE_UPDATE))
                mgr.RemoveLateUpdateFun(behaviourFun[BehaviourFunction.LATE_UPDATE]);
            if (behaviourFun.ContainsKey(BehaviourFunction.FIXED_UPDATE))
                mgr.RemoveFixedUpdateFun(behaviourFun[BehaviourFunction.FIXED_UPDATE]);
        }
    }
}

