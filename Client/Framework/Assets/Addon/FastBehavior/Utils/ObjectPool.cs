using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastBehavior
{
    public class ObjectPool<T> where T : PoolObject
    {
        private Iterator<T> _objList;
        
#if UNITY_EDITOR
        public int maxNum = 0;
        public uint counter = 0;
#endif
        
        public Iterator<T> ObjList
        {
            get => _objList;
        }

        private Queue<T> _objPool;
        
        public Queue<T> ObjPool
        {
            get => _objPool;
        }
        public ObjectPool()
        {
            _objList = new Iterator<T>();
            _objPool = new Queue<T>();
        }

        public T Get()
        {
#if UNITY_EDITOR
            counter ++;
#endif
            var t = Pop();
            t.delete = false;
            _objList.Add(t);
            return t;
        }
        
        public void Store(T t)
        {
            t.delete = true;
            _objList.Remove(t);
            _objPool.Enqueue(t);
#if UNITY_EDITOR
            if (maxNum < _objList.list.Count + _objPool.Count)
                maxNum = _objList.list.Count + _objPool.Count;
#endif
        }

        public bool Contains(PoolObject poolObjectObj)
        {
            return _objList.list.Contains(poolObjectObj as T);
        }
        public bool IsEmpty()
        {
            return _objPool.Count == 0;
        }
        
        private T Pop()
        {
            if (_objPool.Count > 0)
            {
                return _objPool.Dequeue();
            }
            else
            {
                //Debug.LogWarning("StateMachine Pool is overload");
                return Activator.CreateInstance(typeof(T)) as T;
            }
        }
        
        public void Update()
        {
            _objList.Reset();
            while (_objList.MoveNext())
            {
                var sm = _objList.Current as T;
                if (sm != null && !sm.delete)
                {
                    sm.Update();
                }
            }
        }
    }
}