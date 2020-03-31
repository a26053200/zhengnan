using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace FastBehavior
{
    public enum StateOrder
    {
        Join,
        Sequence,
        Select,
    }
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/5/27 23:51:07</para>
    /// </summary> 
    public class StateMachine : PoolObject
    {
        static double s_id = 1;

        public bool isRunning = false;

        public bool delete = false;
        public double id { get; private set; }

        public FastLuaBehavior fastBehavior;
        public List<StateAction> state2DList = new List<StateAction>();
        public StateAction currState { get { return m_currState; } }

        private List<StateAction> m_currstateList = new List<StateAction>();
        private List<StateAction> m_currStateExeList = new List<StateAction>();
        private StateAction m_currState;
        private Queue<StateAction> m_curr2DQueue;
        private LuaFunction m_cycleOverCallback;

        
        public StateMachine()
        {
            id = s_id;
            s_id++;

            //hideFlags = HideFlags.HideInInspector;
            m_currstateList = state2DList;
            isRunning = false;
        }

        public void Run(LuaFunction cycleOverCallback = null)
        {
            m_cycleOverCallback = cycleOverCallback;
            m_currState = null;
            m_curr2DQueue = new Queue<StateAction>(state2DList);
            isRunning = true;
            NextState();
        }

        public void Stop()
        {
            m_currState?.behavior?.Stop();
            m_currState = null;
            isRunning = false;
        }


        public void AppendState(StateAction state)
        {
            m_currstateList.Add(state);
        }

        public void BeginSelect()
        {
            var node = StateMachineManager.GetInstance().CreateStateNode();
            StateAction subState = StateMachineManager.GetInstance().CreateStateAction(node);
            subState.order = StateOrder.Select;
            state2DList.Add(subState);
            m_currstateList = new List<StateAction>();
            subState.AddSubList(m_currstateList);
        }

        public void EndSelect()
        {
            m_currstateList = state2DList;
        }

        public void BeginJoin()
        {
            var node = StateMachineManager.GetInstance().CreateStateNode();
            StateAction subState = StateMachineManager.GetInstance().CreateStateAction(node);
            subState.order = StateOrder.Join;
            state2DList.Add(subState);
            m_currstateList = new List<StateAction>();
            subState.AddSubList(m_currstateList);
        }

        public void EndJoin()
        {
            m_currstateList = state2DList;
        }

        public void NextState()
        {
            if (!isRunning)
                return;
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
            if(m_curr2DQueue.Count == 0)
                return;
            m_currState = null;
            m_currStateExeList = null;
            StateAction state = m_curr2DQueue.Dequeue();
            if (state.order == StateOrder.Sequence)
            {
                m_currState = state;
                m_currState.Start();
            }
            else if (state.order == StateOrder.Select)
            {
                int[] randoms = Utils.GetRandomArray(state.subStateList.Count);
                if (randoms.Length > 0)
                {
                    m_currState = state.subStateList[randoms[0]];
//                    if(m_currState.execute) 
                        m_currState.Start();
                }
            }
            else if (state.order == StateOrder.Join)
            {
                m_currStateExeList = new List<StateAction>(state.subStateList);
                for (int i = 0; i < state.subStateList.Count; i++)
                {
                    state.subStateList[i].Start();
                }
            }
        }

        public override void Update()
        {
            if (!isRunning)
                return;
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
                else
                {
                    if(state.isTimeout)
                        NextState();
                    else 
                        state.Update();
                }
                    
            }else if (m_currStateExeList != null)
            {
                for (int i = 0; i < m_currStateExeList.Count; i++)
                {
                    StateAction state = m_currStateExeList[i];
                    bool isOver = state.IsOver();
                    if (!state.execute)
                    {
                        state.execute = true;
                        state.Execute();
                    }
                    if (!isOver)
                        state.Update();
                }
            }
        }
        public void Pause()
        {
            if(!isRunning)
                return;
            if (m_currState != null)
            {
                m_currState.pauseStartTime = Time.time;
            }else if (m_currStateExeList != null)
            {
                for (int i = 0; i < m_currStateExeList.Count; i++)
                {
                    m_currStateExeList[i].pauseStartTime = Time.time;
                }
            }

            isRunning = false;
        }
        
        public void Resume()
        {
            if(isRunning)
                return;
            if (m_currState != null)
            {
                m_currState.pauseStartTime = Time.time;
                m_currState.startTime += Time.time - m_currState.pauseStartTime;
                m_currState.pauseStartTime = 0;
            }else if (m_currStateExeList != null)
            {
                for (int i = 0; i < m_currStateExeList.Count; i++)
                {
                    m_currStateExeList[i].pauseStartTime = Time.time;
                    m_currStateExeList[i].startTime += Time.time - m_currStateExeList[i].pauseStartTime;
                    m_currStateExeList[i].pauseStartTime = 0;
                }
            }
            isRunning = true;
        }
        public override void Dispose()
        {
            for (int i = 0; i < state2DList.Count; i++)
                state2DList[i].Dispose();
            m_curr2DQueue = null;
            isRunning = false;
            state2DList.Clear();
            m_currstateList = state2DList;
            m_cycleOverCallback?.Dispose();
            StateMachineManager.GetInstance().Store(this);
        }
    }
}
    
