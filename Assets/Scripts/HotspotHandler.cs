using UnityEngine;

public class HotspotHandler : MonoBehaviour
{
    public EHRManager ehrManager;
    public string hotspotName = "Hotspot";
    
    void Start()
    {
        if (ehrManager == null)
        {
            ehrManager = FindObjectOfType<EHRManager>();
            Debug.Log("EHRManager βρέθηκε: " + (ehrManager != null ? "ΝΑΙ" : "ΟΧΙ"));
        }
    }
    
    public void HandleClick()
    {
        Debug.Log("HandleClick στο: " + gameObject.name + " (Hotspot: " + hotspotName + ")");
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("HOTSPOT_INTERACTION", "Κλικ στο: " + hotspotName);
        
        if (ehrManager != null)
        {
            ehrManager.ToggleEHR(gameObject, hotspotName);
        }
        else
        {
            Debug.LogError("EHRManager δεν είναι συνδεδεμένο!");
        }
    }
}