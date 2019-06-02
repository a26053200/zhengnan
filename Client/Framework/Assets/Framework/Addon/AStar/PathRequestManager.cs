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

#if UNITY_EDITOR
        static List<Path> pathList = new List<Path>();
#endif
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

#if UNITY_EDITOR
            for (int i = 0; i < pathList.Count; i++)
            {
                Path path = pathList[i];
                float x = UnityEngine.Random.Range(0f,1f);
                Color color = new Color(path.lookPoints[0].x * 100 % 255, path.lookPoints[0].y * 100 % 255, path.lookPoints[0].z * 100 % 255, 1);
                for (int j = 0; j < path.lookPoints.Length - 1; j++)
                {
                    if (j == 0)
                    {
                        Debug.DrawLine(path.startPos, path.lookPoints[j], color);
                    }
                    else
                    {
                        Debug.DrawLine(path.lookPoints[j], path.lookPoints[j + 1], color);
                    }
                }
            }
#endif
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
        
		public static void RequestPath(Vector3 start, Vector3 end, float turnDst, float stoppingDst, bool smooth, LuaFunction luaFunction, LuaFunction failFuaFunction)
        {
            ThreadStart threadStart = delegate {
				instance.pathfinding.FindPath(new PathRequest(start,end, smooth, delegate(Vector3[] waypoints, bool pathSuccessful)
                {
                    if (pathSuccessful)
                    {
						Path path = new Path(waypoints, start, turnDst, stoppingDst);
						luaFunction.Call(path);
#if UNITY_EDITOR
                        pathList.Add(path);
                        if (pathList.Count > 50)
                        {
                            pathList.RemoveAt(0);
                        }
#endif

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
            this.smooth = smooth;
            callback = _callback;
        }

    }
}

