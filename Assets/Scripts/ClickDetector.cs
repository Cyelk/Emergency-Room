using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
        
        Debug.Log("🖱️ ClickDetector: " + gameObject.name);
        
        HotspotHandler handler = GetComponent<HotspotHandler>();
        if (handler == null) handler = GetComponentInParent<HotspotHandler>();
        
        if (handler != null)
        {
            // ΚΑΛΕΙ ΤΟ HandleClick ΑΝΤΙ ΤΟΥ OnMouseDown
            handler.HandleClick();
        }
        else
        {
            Debug.Log("⚠️ Το " + gameObject.name + " δεν έχει HotspotHandler");
        }
    }
}