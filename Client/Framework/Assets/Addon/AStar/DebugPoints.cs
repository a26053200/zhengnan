using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AStar
{
    public class DebugPoints : MonoBehaviour
    {


        public List<Vector3[]> debugPoints;

        public Color color = Color.yellow;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            for (int i = 0; i < debugPoints.Count; i++)
            {
                for (int j = 0; j < debugPoints[i].Length - 1; j++)
                {
                    Debug.DrawLine(debugPoints[i][j], debugPoints[i][j + 1], color);
                }
                Debug.DrawLine(debugPoints[i][debugPoints[i].Length - 1], debugPoints[i][0], color);
            }
#endif
        }


    }
}

