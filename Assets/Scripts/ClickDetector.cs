using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Αν το Selection Panel είναι ανοιχτό, αγνόησέ το
        if (ScenarioSelectionManager.Instance != null && ScenarioSelectionManager.Instance.IsPanelOpen())
        {
            Debug.Log("Selection ανοιχτό");
            return;
        }
        
        // Αν το Help είναι ανοιχτό, αγνόησέ το
        if (HelpManager.Instance != null && HelpManager.Instance.IsHelpOpen())
        {
            Debug.Log("Help ανοιχτό");
            return;
        }

        eventData.Use();
        
        HotspotHandler handler = GetComponent<HotspotHandler>();
        if (handler == null) handler = GetComponentInParent<HotspotHandler>();
        
        if (handler != null)
            handler.HandleClick();
    }
}