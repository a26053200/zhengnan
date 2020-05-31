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
        
        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOScale(minScale, duration);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOScale(Vector3.one, duration);
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
    }
}