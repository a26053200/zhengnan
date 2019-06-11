using System;

namespace FastBehavior
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/5/27 23:56:27</para>
    /// </summary> 
    public class StateNode
    {
        static double s_id = 1;

        public double id;
        public string name;
        public Action OnEnter;
        public float duration;
        public Action OnUpdate;
        public Action OnExit;

        public StateNode()
        {
            id = s_id++;
        }
    }

    public enum StateOrder
    {
        Parallel,
        Sequence,
        Select,
    }
}
    

