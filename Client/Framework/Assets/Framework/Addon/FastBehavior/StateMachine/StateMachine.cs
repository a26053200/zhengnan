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
    public class StateMachine : MonoBehaviour
    {
        static double s_id = 1;

        private double id;

        public FastLuaBehavior fastBehavior;
        public List<StateAction> state2DList = new List<StateAction>();
        public StateAction currState { get { return m_currState; } }

        private List<StateAction> m_currstateList = new List<StateAction>();
        private List<StateAction> m_currStateExeList = new List<StateAction>();
        private StateAction m_currState;
        private Queue<StateAction> m_curr2DQueue;
        private LuaFunction m_cycleOverCallback;

        
        void Awake()
        {
            id = s_id;
            s_id++;

            //hideFlags = HideFlags.HideInInspector;
            m_currstateList = state2DList;
        }

        public void Run(LuaFunction cycleOverCallback = null)
        {
            m_cycleOverCallback = cycleOverCallback;
            m_currState = null;
            m_curr2DQueue = new Queue<StateAction>(state2DList);
            enabled = true;
            NextState();
        }

        public void Stop()
        {
            m_currState = null;
            enabled = false;
        }


        public void AppendState(StateAction state)
        {
            m_currstateList.Add(state);
        }

        public void BeginScelet()
        {
            StateAction subState = new StateAction(new StateNode());
            subState.order = StateOrder.Select;
            state2DList.Add(subState);
            m_currstateList = new List<StateAction>();
            subState.AddSubList(m_currstateList);
        }

        public void EndScelet()
        {
            m_currstateList = state2DList;
        }

        public void BeginParallel()
        {
            StateAction subState = new StateAction(new StateNode());
            subState.order = StateOrder.Parallel;
            state2DList.Add(subState);
            m_currstateList = new List<StateAction>();
            subState.AddSubList(m_currstateList);
        }

        public void EndParallel()
        {
            m_currstateList = state2DList;
        }

        public void NextState()
        {
            if (m_curr2DQueue.Count == 0)
            {
                m_curr2DQueue = new Queue<StateAction>(state2DList);
                if (m_cycleOverCallback != null )
                {
                    m_cycleOverCallback.BeginPCall();
                    m_cycleOverCallback.PCall();
                    m_cycleOverCallback.EndPCall();
                }
            }
            m_currState = null;
            m_currStateExeList = null;
            StateAction state = m_curr2DQueue.Dequeue();
            if (state.subStateList == null)
            {
                m_currState = state;
                m_currState.Start();
            }
            else if (state.order == StateOrder.Select)
            {
                int[] randoms = Utils.GetRandomArray(state.subStateList.Count);
                m_currState = state.subStateList[randoms[0]];
                m_currState.Start();
            }
            else if (state.order == StateOrder.Parallel)
            {
                m_currStateExeList = new List<StateAction>(state.subStateList);
                for (int i = 0; i < state.subStateList.Count; i++)
                {
                    state.subStateList[i].Start();
                }
            }
        }

        void Update()
        {
            if (m_currState != null)
            {
                StateAction state = m_currState;
                bool isOver = state.IsOver();
                if (!state.execute)
                {
                    state.execute = true;
                    state.Execute();
                }
                if (isOver)
                    NextState();
            }else if (m_currStateExeList != null)
            {
                for (int i = 0; i < m_currStateExeList.Count; i++)
                {
                    StateAction state = m_currStateExeList[i];
                    if (!state.execute)
                    {
                        state.execute = true;
                        state.Execute();
                    }
                }
            }
        }
    }
}
    
