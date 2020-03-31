using System;
using LuaInterface;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework
{
    public class AutoMove : MonoBehaviour
    {
        private Transform _transform;
        private LuaFunction _overCallback;
        private LuaFunction _stepCallback;
        private Vector3 _orgPos;
        private Vector3 _destPos;
        private float _orgY;
        private bool _isAutoMoving;
        private bool _isAutoPath;
        private Vector3 _lastPos;
        
        public float delta;
        
        private void Awake()
        {
            _transform = transform;
        }

        //自动相目标点移动，不需要寻路
        public bool AutoMoveToPos(Vector3 destPos, LuaFunction overCallback, LuaFunction stepCallback = null)
        {
            _overCallback = overCallback;
            _stepCallback = stepCallback;
            _orgPos = transform.position;
            return StartAutoMove(destPos);
        }

        public void Stop()
        {
            _isAutoMoving = false;
            _isAutoPath = false;
        }

        private bool StartAutoMove(Vector3 destPos)
        {
            _destPos = destPos;
            _orgY = transform.position.y;
            _destPos.y = _orgY;
            if (IsArrived(destPos))
            {
                Stop();
                Callback(_overCallback, true);
                return false;
            }

            _isAutoMoving = true;
            return true;
        }

        private void Update()
        {
            if (_isAutoMoving && Time.timeScale > 0)
            {
                var arrived = IsArrived(_destPos);
                if (!arrived)
                {
                    if (_lastPos == _transform.position)
                    {
                        Stop();
                        Callback(_overCallback, false);
                    }

                    var position = _transform.position;
                    _lastPos = position;
                    _transform.position = Vector3.MoveTowards(position, _destPos, delta);
                    if(_lastPos != _transform.position)
                        _transform.forward = (_transform.position - _lastPos).normalized;
                }
                if (arrived)
                {
                    Stop();
                    _transform.position = _destPos;
                    Callback(_overCallback, true);
                }
                else
                    Callback(_stepCallback);
            }
        }

        private bool IsArrived(Vector3 nextPos)
        {
            var distance = Vector3.Distance(transform.position, nextPos);
            return distance <= delta;
        }
        
        private void Callback(LuaFunction callback)
        {
            if (callback != null)
            {
                callback.BeginPCall();
                callback.PCall();
                callback.EndPCall();
            }
        }
        
        private void Callback(LuaFunction callback, bool flag)
        {
            if (callback != null)
            {
                callback.BeginPCall();
                callback.Push(flag);
                callback.PCall();
                callback.EndPCall();
            }
        }

        public void Dispose()
        {
            _isAutoMoving = false;
            _isAutoPath = false;
            if (_overCallback != null)
                _overCallback.Dispose();
            if (_stepCallback != null)
                _stepCallback.Dispose();
            _overCallback = null;
            _stepCallback = null;
        }
    }
}