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
        }
    }
    
    // ΑΦΑΙΡΕΣΕ ΤΟ void OnMouseDown() - ΔΕΝ το χρειαζόμαστε πια!
    // Το ClickDetector θα καλεί τη συνάρτηση HandleClick() αντί για OnMouseDown
    
    // Αυτή τη συνάρτηση θα την καλεί το ClickDetector
    public void HandleClick()
    {
        Debug.Log("🖱️ Κλικ στο: " + gameObject.name + " (Hotspot: " + hotspotName + ")");
        
        if (ehrManager != null)
        {
            ehrManager.ToggleEHR(gameObject, hotspotName);
        }
    }
}