using System;
using System.Collections;
using System.Collections.Generic;

namespace FastBehavior
{
    
    /// <summary>
    /// 迭代器
    /// </summary>
    public class Iterator<T> : IEnumerator
    {
        public List<T> list = new List<T>();
        private object _current = null;
        private int _icout = 0;
        
        public object Current
        {
            get { return _current; }
        }
        
        public bool MoveNext()
        {
            if (_icout >= list.Count)
            {
                return false;
            }
            else
            {
                _current = list[_icout];
                _icout++;
                return true;
            }
        }
 
        public void Reset()
        {
            _icout = 0;
        }
 
        public void Add(T t)
        {
            list.Add(t);
        }

        public void Remove(T t)
        {
            if (list.Contains(t))
            {
                if (list.IndexOf(t) <= _icout)
                {
                    _icout--;
                }

                list.Remove(t);
            }
        }
    }
}