using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListItemEventListener : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ScrollRect scroll;
    
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        scroll.OnInitializePotentialDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scroll.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scroll.OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scroll.OnDrag(eventData);
    }
}
