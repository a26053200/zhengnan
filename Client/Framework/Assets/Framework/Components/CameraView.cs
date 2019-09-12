using UnityEngine;

namespace Framework
{
    public class CameraView : MonoBehaviour
    {
        private Camera _theCamera;

        //距离摄像机8.5米 用黄色表示
        public float upperDistance = 8.5f;

        //距离摄像机12米 用红色表示
        public float lowerDistance = 12.0f;

        public Rect rect;
        
        private Transform tx;

        void Start()
        {
            if (!_theCamera)
            {
                _theCamera = Camera.main;
            }

            if (_theCamera)
                tx = _theCamera.transform;
            FindUpperCorners();
        }


        void Update()
        {
            FindUpperCorners();
            FindLowerCorners();
        }


        void FindUpperCorners()
        {
            Vector3[] corners = GetCorners(upperDistance);
            rect = new Rect(corners[0].x, corners[0].z, Mathf.Abs(corners[1].x - corners[0].x), Mathf.Abs(corners[2].z - corners[0].z));
            
            // for debugging
            Debug.DrawLine(corners[0], corners[1], Color.yellow); // UpperLeft -> UpperRight
            Debug.DrawLine(corners[1], corners[3], Color.yellow); // UpperRight -> LowerRight
            Debug.DrawLine(corners[3], corners[2], Color.yellow); // LowerRight -> LowerLeft
            Debug.DrawLine(corners[2], corners[0], Color.yellow); // LowerLeft -> UpperLeft
        }


        void FindLowerCorners()
        {
            var corners = GetCorners(lowerDistance);
            // for debugging
            Debug.DrawLine(corners[0], corners[1], Color.red);
            Debug.DrawLine(corners[1], corners[3], Color.red);
            Debug.DrawLine(corners[3], corners[2], Color.red);
            Debug.DrawLine(corners[2], corners[0], Color.red);
        }


        Vector3[] GetCorners(float distance)
        {
            Vector3[] corners = new Vector3[4];

            float halfFOV = (_theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            float aspect = _theCamera.aspect;

            float height = distance * Mathf.Tan(halfFOV);
            float width = height * aspect;

            // UpperLeft
            corners[0] = tx.position - (tx.right * width);
            corners[0] += tx.up * height;
            corners[0] += tx.forward * distance;

            // UpperRight
            corners[1] = tx.position + (tx.right * width);
            corners[1] += tx.up * height;
            corners[1] += tx.forward * distance;

            // LowerLeft
            corners[2] = tx.position - (tx.right * width);
            corners[2] -= tx.up * height;
            corners[2] += tx.forward * distance;

            // LowerRight
            corners[3] = tx.position + (tx.right * width);
            corners[3] -= tx.up * height;
            corners[3] += tx.forward * distance;

            return corners;
        }
    }
}