using System;
using LuaInterface;

namespace FastBehavior
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/5/27 23:56:27</para>
    /// </summary> 
    public class StateNode : PoolObject
    {
        static double s_id = 1;

        public double id;
        public string name;
        public Action OnEnterAction;
        public LuaFunction OnEnter;
        public float duration;
        public LuaFunction OnUpdate;
        public LuaFunction OnExit;
        

        public StateNode()
        {
            id = s_id++;
        }

        public void OnEnterDelegate()
        {
            if (OnEnter != null)
            {
                OnEnter.BeginPCall();
                OnEnter.PCall();
                OnEnter?.EndPCall();
            }
            
        }
        
        public void OnUpdateDelegate()
        {
            if (OnUpdate != null)
            {
                OnUpdate.BeginPCall();
                OnUpdate.PCall();
                OnUpdate?.EndPCall();
            }
        }
        
        public void OnExitDelegate()
        {
            if (OnExit != null)
            {
                OnExit.BeginPCall();
                OnExit.PCall();
                OnExit?.EndPCall();
            }
        }

        public override void Dispose()
        {
            duration = 0;
            OnEnter?.EndPCall();
            OnEnter = null;
            OnUpdate?.EndPCall();
            OnUpdate = null;
            OnExit?.EndPCall();
            OnExit = null;
            StateMachineManager.GetInstance().Store(this);
        }
    }

    
}
    

