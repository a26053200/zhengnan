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
    public class StateAction
    {
        private StateNode m_node;
        private float m_startTime;

        public StateOrder order { get; set; }
        public List<StateAction> subStateList { get; private set; }

        public bool execute { get; set; }

        public StateAction(StateNode node)
        {
            m_node = node;
            m_startTime = Time.time;
            execute = false;
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
            if (m_node.OnUpdate != null)
                m_node.OnUpdate();
            
        }

        public void Execute()
        {
            if (m_node.OnEnter != null)
                m_node.OnEnter();
        }

        public bool IsOver()
        {
            if (m_node.duration > 0)
                return Time.time - m_startTime > m_node.duration;
            else
                return false;
        }

    }
}
