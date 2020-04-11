using System;
using DG.Tweening;
using LuaInterface;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework
{
    /// <summary>
    /// 相机追踪
    /// </summary>
    [ExecuteInEditMode]
    public class AttachCamera : MonoBehaviour
    {
        public float attachMinDistance = 0.1f;
        public bool isAttaching;
        public bool isSmooth;
        public Camera tagCamera;
        public GameObject target;
        public Vector3 targetPos;
        public Vector3 offset;
        public float updateSpeed = 0.1f;
        //public float scale = 0.8f;
        public bool noShake;
        public float radius  = 12f;
        //public float roundY = 0f;
        public Vector3 angle = Vector3.zero;

        //private Vector3 _lastTagPos = Vector3.positiveInfinity;
        private Vector3 _tempTagPos;
        
        private float _orgRadius;
        private Vector3 _orgAngle;
        private float _orgUpdateSpeed;

        //private float _tempScale = 1;
        private Transform _targetTransform;

        private Transform targetTransform
        {
            get
            {
                if (!_targetTransform && target)
                    _targetTransform = target.transform;
                return _targetTransform;
            }
        }
        private Transform _cameraTransform;
        private Transform cameraTransform
        {
            get
            {
                if (!_cameraTransform && tagCamera)
                    _cameraTransform = tagCamera.transform;
                return _cameraTransform;
            }
        }
        private LuaFunction _attachCallback;
       
        private Sequence _shakeTween;

        public void Init(Camera camera)
        {
            this.tagCamera = camera;
            _cameraTransform = camera.transform;
        }
        public void Attach(GameObject target)
        {
            this.target = target;
            _targetTransform = target.transform;
            isAttaching = target != null;
        }
        public void AttachPos(Vector3 pos, LuaFunction attachCallback)
        {
            _attachCallback = attachCallback;
            target = null;
            targetPos = pos;
            isAttaching = true;
        }

        public void Reset()
        {
            _tempTagPos = GetTargetPos();
            cameraTransform.position = GetCameraPos(_tempTagPos) + offset;
            cameraTransform.forward = (_tempTagPos - cameraTransform.position).normalized;
        }
        private void LateUpdate()
        {
            if (isAttaching)
            {
                _tempTagPos = GetTargetPos();
//                _tempScale = 1;
//                if (_lastTagPos != Vector3.positiveInfinity)
//                    _tempScale = (Vector3.Distance(_tempTagPos, _lastTagPos) / Time.timeScale) * scale;
                var camDestPos = GetCameraPos(_tempTagPos);
                if (isSmooth)
                {
                    var position = cameraTransform.position;
                    position = Vector3.Lerp(position, camDestPos + offset, updateSpeed);
                    cameraTransform.position = position;
                    cameraTransform.forward = Vector3.Lerp(cameraTransform.forward, (_tempTagPos - camDestPos).normalized, updateSpeed);
                }
                else
                {
                    cameraTransform.position = camDestPos + offset;
                    cameraTransform.forward = (_tempTagPos - camDestPos).normalized;
                }
//                _lastTagPos = _tempTagPos;
                if (_attachCallback != null)
                {
                    if (attachMinDistance > Vector3.Distance(cameraTransform.position, camDestPos))
                    {
                        _attachCallback.BeginPCall();
                        _attachCallback.PCall();
                        _attachCallback.EndPCall();
                        _attachCallback = null;
                    }
                }
            }
        }

        public void ShakeCamera( float delay,float duration,Vector3 strength,int vibrato = 50, float randomness = 0.5f,bool fadeOut = false)
        {
            if(noShake)
                return;
            _shakeTween?.Kill();
            _shakeTween = DOTween.Sequence();
            if (delay > 0)
                _shakeTween.AppendInterval(delay);
            _shakeTween.Append(tagCamera.DOShakePosition(duration,strength,vibrato,randomness,fadeOut));
        }

        public void ShakeCameraStop()
        {
            _shakeTween?.Kill();
            _shakeTween = null;
            tagCamera.DOPause();
        }
        //特写镜头
        public void CloseupShot(float radius, Vector3 angle, float updateSpeed, LuaFunction callback)
        {
            _orgRadius = this.radius;
            _orgAngle = this.angle;
            _orgUpdateSpeed = this.updateSpeed;
            
            this.radius = radius;
            this.angle = angle;
            this.updateSpeed = updateSpeed;
            _attachCallback = callback;
        }

        public void Resume()
        {
            radius = _orgRadius;
            angle = _orgAngle;
            updateSpeed = _orgUpdateSpeed;
        }

        private Vector3 GetTargetPos()
        {
            if (targetTransform)
                return targetTransform.position;
            else
                return targetPos;
        }
        private Vector3 GetCameraPos(Vector3 tagPos)
        {
            Vector3 camPos = tagPos;
            Vector3 back = Quaternion.Euler(0,angle.y,0) * Vector3.back;
            camPos += back * (radius * Mathf.Cos(angle.x * Mathf.Deg2Rad));
            camPos += Vector3.up * (radius * Mathf.Sin(angle.x * Mathf.Deg2Rad));
            return camPos;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (targetTransform && cameraTransform)
            {
                Vector3 back = Quaternion.Euler(0,angle.y,0) * Vector3.back;
                var tempPos0 = _tempTagPos + back * (radius * Mathf.Cos(angle.x * Mathf.Deg2Rad));
                var tempPos1 = tempPos0 + Vector3.up * (radius * Mathf.Sin(angle.x * Mathf.Deg2Rad));
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_tempTagPos, cameraTransform.position);
                Gizmos.DrawLine(_tempTagPos, tempPos0);
                Gizmos.DrawLine(tempPos0, tempPos1);
            }
        }
#endif
        public void Dispose()
        {
            _attachCallback = null;
        }
    }
    
}