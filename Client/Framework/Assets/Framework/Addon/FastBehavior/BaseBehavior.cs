using System;
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
    
    public class BaseBehavior
    {
        static long s_id = 1;
        public long id { get; private set; }

        private StateMachine m_machine;
        private List<BaseBehavior> m_subBehaviors;

        public BaseBehavior(StateMachine machine)
        {
            id = s_id;
            s_id++;

            m_machine = machine;
            m_subBehaviors = new List<BaseBehavior>();
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


        public void AppendBehavior(BaseBehavior behavior)
        {
            StateNode node = new StateNode();
            node.name = "Sub BaseBehavior id:" + behavior.id;
            m_subBehaviors.Add(behavior);
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
            m_machine.AppendState(new StateAction(node));
        }


        public void Run(LuaFunction cycleOverCallback = null)
        {
            m_machine.Run(cycleOverCallback);
        }

        public void Stop()
        {
            m_machine.Stop();
        }

        public void NextState()
        {
            m_machine.NextState();
        }
    }
}

