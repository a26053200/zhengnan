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
    public class FastLuaBehavior
    {
        static double s_id = 1;
        public double id { get; private set; }

        public StateMachine stateMachine { get; private set; }

        public List<FastLuaBehavior> subBehaviors { get; private set; }

        public FastLuaBehavior(StateMachine machine)
        {
            id = s_id;
            s_id++;

            stateMachine = machine;
            machine.fastBehavior = this;
            subBehaviors = new List<FastLuaBehavior>();
        }

        public void AppendState(LuaFunction onEnter, string name)
        {
            StateNode node = new StateNode();
            node.name = name;
            node.OnEnter = delegate ()
            {
                onEnter.BeginPCall();
                onEnter.PCall();
                onEnter.EndPCall();
            };
            AppendStateNode(node);
        }

        public void AppendState(LuaFunction onEnter, LuaFunction onUpdate, string name)
        {
            StateNode node = new StateNode();
            node.name = name;
            if (onEnter != null)
            {
                node.OnEnter = delegate ()
                {
                    onEnter.BeginPCall();
                    onEnter.PCall();
                    onEnter.EndPCall();
                };
            }
            if (onUpdate != null)
            {
                node.OnUpdate = delegate ()
                {
                    onUpdate.BeginPCall();
                    onUpdate.PCall();
                    onUpdate.EndPCall();
                };
            }
            AppendStateNode(node);
        }


        public void AppendBehavior(FastLuaBehavior behavior)
        {
            StateNode node = new StateNode();
            node.name = "Sub BaseBehavior id:" + behavior.id;
            subBehaviors.Add(behavior);
            node.OnEnter = delegate ()
            {
                behavior.Run();
            };
            AppendStateNode(node);
        }

        public void AppendInterval(float interval)
        {
            StateNode node = new StateNode();
            node.name = "AppendInterval interval:" + interval;
            node.duration = interval;
            AppendStateNode(node);
        }

        public void AppendStateNode(StateNode node)
        {
            stateMachine.AppendState(new StateAction(node));
        }


        public void Run(LuaFunction cycleOverCallback = null)
        {
            stateMachine.Run(cycleOverCallback);
        }

        public void Stop()
        {
            stateMachine.Stop();
        }

        public void NextState()
        {
            stateMachine.NextState();
        }
    }
}
    
