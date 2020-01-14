using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using LuaInterface;

namespace AStar
{
	public class PathRequestManager : MonoBehaviour
    {

        Queue<PathResult> results = new Queue<PathResult>();

        static PathRequestManager instance;
        Pathfinding pathfinding;

        void Awake()
        {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        void Update()
        {
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }
        }

        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate {
                instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }

        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }

        [NoToLua]
        public static void RequestPath(Vector3 start, Vector3 end, float turnDst, float stoppingDst, bool smooth, Action<Path> success, Action fail)
        {
            ThreadStart threadStart = delegate {
                instance.pathfinding.FindPath(new PathRequest(start,end, smooth, delegate(Vector3[] waypoints, bool pathSuccessful)
                {
                    if (pathSuccessful)
                    {
                        Path path = new Path(waypoints, start, turnDst, stoppingDst);
                        success(path);
                    }
                    else
                    {
                        fail();
                    }
                }), instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }
        
		public static void RequestPath(Vector3 start, Vector3 end, float turnDst, float stoppingDst, bool smooth, LuaFunction luaFunction, LuaFunction failFuaFunction)
        {
            ThreadStart threadStart = delegate {
				instance.pathfinding.FindPath(new PathRequest(start,end, smooth, delegate(Vector3[] waypoints, bool pathSuccessful)
                {
                    if (pathSuccessful)
                    {
						Path path = new Path(waypoints, start, turnDst, stoppingDst);
						luaFunction.Call(path);
                    }
                    else
                    {
                        failFuaFunction.Call();
                    }
                }), instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }

    }

    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }

    }

    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public bool smooth;

        public PathRequest(Vector3 _start, Vector3 _end, bool smooth, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            this.smooth = smooth;
        }

    }
}

