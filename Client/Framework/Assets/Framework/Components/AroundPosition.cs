using System;
using System.Collections.Generic;
using System.Globalization;
using AStar;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 围绕单位的点
    /// </summary>
    public class AroundPosition : MonoBehaviour
    {
        private bool _dirty;
        private Dictionary<string, AroundPoint[]> _aroundMap;
        private float _y;
        
        public AStar.Grid grid;

        private void Awake()
        {
            _aroundMap = new Dictionary<string, AroundPoint[]>();
        }

        public void UpdateY()
        {
            _y = transform.position.y;
        }
        public void SetDirty(bool dirty)
        {
            _dirty = dirty;
        }

        public AroundPoint[] GetAroundPoints(float gap, int aroundMaxNum)
        {
            string key = gap.ToString(CultureInfo.InvariantCulture);
            if (!_aroundMap.TryGetValue(key, out var positions))
            {
                positions = new AroundPoint[aroundMaxNum];
                for (int i = 0; i < aroundMaxNum; i++)
                {
                    positions[i] = new AroundPoint()
                    {
                        pos = Vector3.zero
                    };
                }
                UpdateAroundPositions(gap, positions);
                _aroundMap.Add(key, positions);
            }

            if (_dirty)
            {
                //_dirty = false;
                UpdateAroundPositions(gap, positions);
            }
            return positions;
        }

        private void UpdateAroundPositions(float gap, AroundPoint[] positions)
        {
            float radian = 2 * Mathf.PI / positions.Length;
            var c = transform.position;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].pos.Set(
                    c.x + gap * Mathf.Cos(radian * i),
                    _y,
                    c.z + gap * Mathf.Sin(radian * i));
                if (grid)
                {
                    positions[i].node = grid.NodeFromWorldPoint(positions[i].pos);
                    positions[i].walkable = positions[i].node.walkable;
                }
                positions[i].owner = 0;
            }
        }

        public void Dispose()
        {
            Destroy(this);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var positions in _aroundMap.Values)
            {
                var last = positions[0].pos;
                for (int i = 0; i < positions.Length; i++)
                {
                    Gizmos.color = positions[i].walkable ? Color.green : Color.red;
                    if(positions[i].owner > 0)
                        Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(last, positions[i].pos);
                    last = positions[i].pos;
                }
                Gizmos.DrawLine(last, positions[0].pos);
            }
        }
#endif
    }
    
    public class AroundPoint
    {
        public int owner;
        public Vector3 pos;
        public bool walkable;
        public Node node;
    }
}