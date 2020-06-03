using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    /// <summary>
    /// <para>点击反馈</para>
    /// <para>Author: zhengnan </para>
    /// <para>Create: DATE TIME</para>
    /// </summary> 
    public class ClickFeedback : MonoBehaviour, 
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,IPointerEnterHandler, IPointerExitHandler
    {
        public Vector3 minScale = new Vector3(0.95f,0.95f,0.95f);
        public float duration = 0.18f;

        private Tween _tween;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _tween = transform.DOScale(minScale, duration);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _tween = transform.DOScale(Vector3.one, duration);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOPause();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}