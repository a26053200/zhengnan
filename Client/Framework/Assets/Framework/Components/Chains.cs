//
// 链
// Author: zhengnan
// Create: 2018/6/8 14:00:27
// 

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    ///<summary>
    /// uv贴图闪电链
    ///</summary>
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    public class Chains : MonoBehaviour
    {
        [Tooltip("最小线段数量")]
        [Range(1, 30)]
        public float minLineNum = 1;//
        [Tooltip("最大线段数量")]
        [Range(1, 30)]
        public float maxLineNum = 6;//
        [Tooltip("线段抖动振幅")]
        public float shake = 2.0f;
        [Tooltip("震动频率")]
        [Range(1, 60)]
        public float rate = 30;

        public Transform target;//链接目标
        public Transform start;
        public float yOffset = 0;

        private LineRenderer _lineRender;
        private List<Vector3> _linePosList;
        private Vector3 _startPos = Vector3.zero;
        private Vector3 _endPos = Vector3.zero;
        private float _startTime = 0;

        private void Awake()
        {
            _lineRender = gameObject.GetComponent<LineRenderer>();
            _linePosList = new List<Vector3>();
        }

        private void Update()
        {
            if(Time.time - _startTime > 1 / rate)
            {
                _startTime = Time.time;
                if (Time.timeScale != 0)
                {
                    if (target != null && start != null)
                    {
                        _endPos = target.position + Vector3.up * yOffset;
                        _startPos = start.position + Vector3.up * yOffset;
                    }
                    else
                    {
                        return;
                    }
                    _linePosList.Clear();
                    GenerateLerpPoints(_startPos, _endPos, maxLineNum, shake);
                    _linePosList.Add(_endPos);

                    _lineRender.positionCount = _linePosList.Count;
                    for (int i = 0, n = _linePosList.Count; i < n; i++)
                    {
                        _lineRender.SetPosition(i, _linePosList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 递归生成中间差值点
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="destPos"></param>
        /// <param name="displace"></param>
        private void GenerateLerpPoints(Vector3 startPos, Vector3 destPos, float maxLineNum, float shake)
        {
            if (maxLineNum < minLineNum)
            {
                _linePosList.Add(startPos);
            }
            else
            {

                float midX = (startPos.x + destPos.x) / 2;
                float midY = (startPos.y + destPos.y) / 2;
                float midZ = (startPos.z + destPos.z) / 2;

                midX += (float)(UnityEngine.Random.value - 0.5) * shake;
                midY += (float)(UnityEngine.Random.value - 0.5) * shake;
                midZ += (float)(UnityEngine.Random.value - 0.5) * shake;

                Vector3 midPos = new Vector3(midX, midY, midZ);

                GenerateLerpPoints(startPos, midPos, maxLineNum / 2, shake / 2);
                GenerateLerpPoints(midPos, destPos, maxLineNum / 2, shake / 2);
            }
        }


    }   

}
