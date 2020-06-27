using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FastBehavior
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/5/27 23:51:07</para>
    /// </summary> 
    public class StateAction : PoolObject
    {
        private StateNode m_node;
        private float m_startTime;
        private float m_timeout;

        public StateNode node { get { return m_node; } }
        public StateOrder order { get; set; }
        public List<StateAction> subStateList { get; private set; }

        public float startTime
        {
            get { return m_startTime; }
            set { m_startTime = value; }
        }

        public float pauseStartTime;
        public FastLuaBehavior parentBehavior;
        
        public FastLuaBehavior behavior;
        
        public bool execute { get; set; }

        public bool isTimeout { get; private set; }

        public StateAction(float timeout)
        {
            m_timeout = timeout;
            order = StateOrder.Sequence;
        }
        public void SetStateAction(StateNode node)
        {
            m_node = node;
            m_startTime = Time.time;
            execute = false;
            isTimeout = false;
            order = StateOrder.Sequence;
        }

        public void AddSubList(List<StateAction> subStateList)
        {
            this.subStateList = subStateList;
        }

        public void Start()
        {
            m_startTime = Time.time;
            execute = false;
        }

        public void Update()
        {
            if (execute && m_startTime > 0 && Time.time - m_startTime > m_timeout)
            {
                Debug.LogErrorFormat("State Machine time out [{0}]", m_node.name);
                isTimeout = true;
            }
            m_node.OnUpdateDelegate();
        }

        public void Execute()
        {
            m_node.OnEnterDelegate();
            if (behavior != null)
            {
                if (order == StateOrder.Join)
                {
                    behavior.Run();
                    behavior.Update();
                    behavior.Stop();
                }
                else
                {
                    if (parentBehavior != null)
                    {
                        parentBehavior.lastBehavior?.Stop();
                        behavior.Run();
                        parentBehavior.lastBehavior = behavior;
                    }
                }
            }
        }

        public bool IsOver()
        {
            if (m_node.duration > 0)
                return Time.time - m_startTime > m_node.duration;
            else
                return false;
        }

        public override void Dispose()
        {
            m_startTime = 0;
            execute = false;
            isTimeout = false;
            behavior?.Stop();
            behavior = null;
            parentBehavior = null;
            order = StateOrder.Sequence;
            subStateList?.Clear();
            m_node.Dispose();
            StateMachineManager.GetInstance().Store(this);
        }
    }
}
