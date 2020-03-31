using System;
using DG.Tweening;
using LuaInterface;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 相机追踪
    /// </summary>
    public class AttachCamera : MonoBehaviour
    {
        public float attachMinDistance = 0.1f;
        public bool isAttaching;
        public bool isSmooth;
        public Camera camera;
        public GameObject target;
        public Vector3 targetPos;
        public Vector3 offset;
        public float updateSpeed = 0.1f;
        //public float scale = 0.8f;
        public bool noShake;
        public float radius  = 12f;
        //public float roundY = 0f;
        public float angle = 30f;

        //private Vector3 _lastTagPos = Vector3.positiveInfinity;
        private Vector3 _tempTagPos;
        
        private float _orgRadius;
        private float _orgAngle;
        private float _orgUpdateSpeed;

        //private float _tempScale = 1;
        private Transform _targetTransform;
        private Transform _cameraTransform;
        private LuaFunction _attachCallback;
       
        private Sequence _shakeTween;

        public void Init(Camera camera)
        {
            this.camera = camera;
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
            _cameraTransform.position = GetCameraPos(_tempTagPos) + offset;
            _cameraTransform.forward = (_tempTagPos - _cameraTransform.position).normalized;
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
                    var position = _cameraTransform.position;
                    position = Vector3.Lerp(position, camDestPos + offset, updateSpeed);
                    _cameraTransform.position = position;
                    _cameraTransform.forward = Vector3.Lerp(_cameraTransform.forward, (_tempTagPos - camDestPos).normalized, updateSpeed);
                }
                else
                {
                    _cameraTransform.position = camDestPos + offset;
                    _cameraTransform.forward = (_tempTagPos - camDestPos).normalized;
                }
//                _lastTagPos = _tempTagPos;
                if (_attachCallback != null)
                {
                    if (attachMinDistance > Vector3.Distance(_cameraTransform.position, camDestPos))
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
            _shakeTween.Append(camera.DOShakePosition(duration,strength,vibrato,randomness,fadeOut));
        }

        public void ShakeCameraStop()
        {
            _shakeTween?.Kill();
            _shakeTween = null;
            camera.DOPause();
        }
        //特写镜头
        public void CloseupShot(float radius, float angle, float updateSpeed, LuaFunction callback)
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
            if (target)
                return _targetTransform.position;
            else
                return targetPos;
        }
        private Vector3 GetCameraPos(Vector3 tagPos)
        {
            Vector3 camPos = tagPos;
            camPos += Vector3.back * (radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            camPos += Vector3.up * (radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            return camPos;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_targetTransform && _cameraTransform)
            {
                var tempPos0 = _tempTagPos + Vector3.back * (radius * Mathf.Cos(angle * Mathf.Deg2Rad));
                var tempPos1 = tempPos0 + Vector3.up * (radius * Mathf.Sin(angle * Mathf.Deg2Rad));
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_tempTagPos, _cameraTransform.position);
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