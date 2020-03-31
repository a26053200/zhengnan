using System.Collections.Generic;
using System.Linq;
using AStar;
using LuaInterface;
using UnityEngine;
using Grid = AStar.Grid;

namespace Framework
{
    public class AutoPath : MonoBehaviour
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
        private Node _currNode;
        private Node _destNode;
        private List<Vector3> _pathList;
        private Queue<Vector3> _pathQueue;
        
        public float delta;
        public Grid grid;
        
        private void Awake()
        {
            _transform = transform;
            _pathList = new List<Vector3>();
            _pathQueue = new Queue<Vector3>();
        }

        //自动相目标点移动，不需要寻路
        public bool AutoMoveToPos(Vector3 destPos, LuaFunction overCallback, LuaFunction stepCallback = null, bool smooth = false)
        {
            _overCallback = overCallback;
            _stepCallback = stepCallback;
            _orgPos = transform.position;
            return StartAutoMove(destPos, smooth);
        }

        public void Stop()
        {
            _isAutoMoving = false;
            _isAutoPath = false;
        }

        private bool StartAutoMove(Vector3 destPos, bool smooth)
        {
            _destPos = destPos;
            var position = _transform.position;
            _orgY = position.y;
            _destPos.y = _orgY;

            _currNode = grid.NodeFromWorldPoint(position);
            _destNode = grid.NodeFromWorldPoint(destPos);
            if (!_destNode.walkable)
            {
                Callback(_overCallback, false);
                return false;
            }
            if (IsArrived(destPos) || EqualNode(_destNode, _currNode))
            {
                Stop();
                Callback(_overCallback, true);
                return false;
            }
            AStar.PathRequestManager.RequestPath(position, _destPos, 5, 10, smooth, OnPathFound, OnPathFail);
            return true;
        }

        private void OnPathFound(Path path)
        {
            if (path.lookPoints.Length > 0)
            {
                _pathList.Clear();
                _pathList.Add(transform.position);
                _pathList.AddRange(path.lookPoints);
                _pathQueue = new Queue<Vector3>(_pathList);
                if (path.lookPoints[path.lookPoints.Length - 1] != _destPos)
                    _pathQueue.Enqueue(_destPos);
                _isAutoMoving = true;
            }
            else
            {
                Callback(_overCallback, false);
            }
        }
        
        private void OnPathFail()
        {
            _isAutoMoving = false;
            if (NodeDistance(_currNode, _destNode) == 1)
            {
                _pathList.Clear();
                _pathList.Add(transform.position);
                _pathQueue = new Queue<Vector3>(_pathList);
                if (_destNode.worldPosition != _destPos)
                    _pathQueue.Enqueue(_destPos);
                _isAutoMoving = true;
            }
            else
            {
                Callback(_overCallback, false);
            }
        }
        
        private void Update()
        {
            if (_isAutoMoving && Time.timeScale > 0)
            {
                var nextPos = _pathQueue.Peek();
                var arrived = IsArrived(nextPos);
                if (!arrived)
                {
                    if (_lastPos == _transform.position)
                    {
                        Stop();
                        Callback(_overCallback, false);
                    }

                    var position = _transform.position;
                    _lastPos = position;
                    _transform.position = Vector3.MoveTowards(position, nextPos, delta);
                    if(_lastPos != _transform.position)
                        _transform.forward = (_transform.position - _lastPos).normalized;
                }
                if (arrived)
                {
                    if (_pathQueue.Count == 1)
                    {
                        Stop();
                        _transform.position = _destPos;
                        Callback(_overCallback, true);
                    }
                    else
                    {
                        Callback(_stepCallback);
                        _pathQueue.Dequeue();
                    }
                        
                }
            }
        }
        
        private bool IsArrived(Vector3 nextPos)
        {
            var distance = Vector3.Distance(transform.position, nextPos);
            return distance <= delta;
        }
        
        private bool EqualNode(Node node1, Node node2)
        {
            return node1.gridX == node2.gridX && node1.gridY == node2.gridY;
        }
        
        private int NodeDistance(Node node1, Node node2)
        {
            var dx = Mathf.Abs(node1.gridX - node2.gridX);
            var dy = Mathf.Abs(node1.gridY - node2.gridY);
            var d = Mathf.Max(dx, dy);
            return d;
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