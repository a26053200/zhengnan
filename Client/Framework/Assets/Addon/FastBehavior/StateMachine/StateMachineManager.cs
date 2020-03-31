using System;
using LuaInterface;
using UnityEngine;

namespace FastBehavior
{
    public class StateMachineManager : MonoBehaviour
    {
        private static StateMachineManager s_instance;
        
        public static StateMachineManager GetInstance()
        {
            if (s_instance == null)
            {
                GameObject obj = new GameObject("[FSM]");
                DontDestroyOnLoad(obj);
                s_instance = obj.AddComponent<StateMachineManager>();
            }
            return s_instance;
        }
        
        [HideInInspector]
        [NoToLua]
        public ObjectPool<StateMachine> StateMachinePool;
        [HideInInspector]
        [NoToLua]
        public ObjectPool<FastLuaBehavior> FastLuaBehaviorPool;
        [HideInInspector]
        [NoToLua]
        public ObjectPool<StateAction> StateActionPool;
        [HideInInspector]
        [NoToLua]
        public ObjectPool<StateNode> StateNodePool;
        //状态最大超时时间
        public float StateActionTimeout = 10f;
        
        public void Init()
        {
            StateMachinePool = new ObjectPool<StateMachine>();
            FastLuaBehaviorPool = new ObjectPool<FastLuaBehavior>();
            StateActionPool = new ObjectPool<StateAction>();
            StateNodePool = new ObjectPool<StateNode>();
        }

        public StateMachine CreateStateMachine()
        {
            if (StateMachinePool.IsEmpty())
            {
                //Debug.LogWarning("StateMachine Pool is overload");
                var sm = new StateMachine();
                StateMachinePool.Store(sm);
            }
            return StateMachinePool.Get();
        }
        
        public FastLuaBehavior CreateFastLuaBehavior(StateMachine sm)
        {
            if (FastLuaBehaviorPool.IsEmpty())
            {
                //Debug.LogWarning("StateMachine Pool is overload");
                var b = new FastLuaBehavior();
                FastLuaBehaviorPool.Store(b);
            }
            var o = FastLuaBehaviorPool.Get();
            o.SetStateMachine(sm);
            return o;
        }
        
        public StateAction CreateStateAction(StateNode node)
        {
            if (StateActionPool.IsEmpty())
            {
                //Debug.LogWarning("StateMachine Pool is overload");
                var sa = new StateAction(StateActionTimeout);
                StateActionPool.Store(sa);
            }
            var o = StateActionPool.Get();
            o.SetStateAction(node);
            return o;
        }
        
        public StateNode CreateStateNode()
        {
            if (StateNodePool.IsEmpty())
            {
                //Debug.LogWarning("StateMachine Pool is overload");
                var sa = new StateNode();
                StateNodePool.Store(sa);
            }
            return StateNodePool.Get();
        }

        public void Store(PoolObject poolObj)
        {
            if (StateMachinePool.Contains(poolObj))
                StateMachinePool.Store(poolObj as StateMachine);
            else if (FastLuaBehaviorPool.Contains(poolObj))
                FastLuaBehaviorPool.Store(poolObj as FastLuaBehavior);
            else if (StateActionPool.Contains(poolObj))
                StateActionPool.Store(poolObj as StateAction);
            else if (StateNodePool.Contains(poolObj))
                StateNodePool.Store(poolObj as StateNode);
        }
        
        private void Update()
        {
            if (Time.timeScale > 0)
            {
                if(StateMachinePool != null)
                    StateMachinePool.Update();
            }
        }
    }
}