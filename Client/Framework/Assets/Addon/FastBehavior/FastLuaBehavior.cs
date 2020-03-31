using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace FastBehavior
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/5/27 23:51:07</para>
    /// </summary> 
    public class FastLuaBehavior : PoolObject
    {
        static double s_id = 1;
        public double id { get; private set; }

        public StateNode parentNode { get; private set; }

        public StateMachine stateMachine { get; private set; }

        //public List<FastLuaBehavior> subBehaviors { get; private set; }

        public FastLuaBehavior lastBehavior;
        public FastLuaBehavior()
        {
            //subBehaviors = new List<FastLuaBehavior>();
        }

        public void SetStateMachine(StateMachine machine)
        {
            id = s_id;
            s_id++;

            stateMachine = machine;
            machine.fastBehavior = this;
        }

        public void AppendState(LuaFunction onEnter, string name)
        {
            StateNode node = StateMachineManager.GetInstance().CreateStateNode();
            node.name = name;
            node.OnEnter = onEnter;
            AppendStateNode(node);
        }

        public void AppendState(LuaFunction onEnter, LuaFunction onUpdate, string name)
        {
            StateNode node = StateMachineManager.GetInstance().CreateStateNode();
            node.name = name;
            node.OnEnter = onEnter;
            node.OnUpdate = onUpdate;
            AppendStateNode(node);
        }


        public void AppendBehavior(FastLuaBehavior behavior, string name)
        {
            StateNode node = StateMachineManager.GetInstance().CreateStateNode();
            node.name = name;
            var action = StateMachineManager.GetInstance().CreateStateAction(node);
            action.parentBehavior = this;
            action.behavior = behavior;
            behavior.parentNode = node;
            stateMachine.AppendState(action);
        }


        public void JoinBehavior(FastLuaBehavior behavior, string name)
        {
            StateNode node = StateMachineManager.GetInstance().CreateStateNode();
            node.name = name;
            var action = StateMachineManager.GetInstance().CreateStateAction(node);
            action.parentBehavior = this;
            action.behavior = behavior;
            behavior.parentNode = node;
            stateMachine.AppendState(action);
        }

        public void AppendInterval(float interval)
        {
            StateNode node = StateMachineManager.GetInstance().CreateStateNode();
            node.name = "AppendInterval interval:" + interval;
            node.duration = interval;
            AppendStateNode(node);
        }

        private void AppendStateNode(StateNode node)
        {
            var action = StateMachineManager.GetInstance().CreateStateAction(node);
            stateMachine.AppendState(action);
        }
        public void Run(LuaFunction cycleOverCallback = null)
        {
            stateMachine.Run(cycleOverCallback);
        }

        public void Pause()
        {
            stateMachine.Pause();
        }
        
        public void Resume()
        {
            stateMachine.Resume();
        }
        
        public void Stop()
        {
            lastBehavior?.Stop();
            lastBehavior = null;
            stateMachine.Stop();
        }

        public void NextState()
        {
            stateMachine.NextState();
        }

        public override void Dispose()
        {
            stateMachine.Dispose();
            stateMachine = null;
            lastBehavior = null;
            parentNode = null;
            StateMachineManager.GetInstance().Store(this);
        }
    }
}
    
