using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 10f;
    
    private Camera playerCamera;
    private EHRManager ehrManager;
    
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        ehrManager = FindObjectOfType<EHRManager>();
        
        Debug.Log("🎥 PlayerInteraction ξεκίνησε. Κάμερα: " + (playerCamera != null));
        Debug.Log("🔍 EHRManager: " + (ehrManager != null ? "Βρέθηκε" : "ΔΕΝ βρέθηκε"));
    }
    
    void Update()
    {
        // Αν το EHR είναι ανοιχτό, μην κάνεις interact
        if (ehrManager != null && ehrManager.IsEHRPanelOpen())
        {
            return;
        }
        
        // Κάνε raycast από το κέντρο της οθόνης
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
        
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            // Ψάξε για HotspotHandler
            HotspotHandler handler = hitObject.GetComponent<HotspotHandler>();
            if (handler == null) handler = hitObject.GetComponentInParent<HotspotHandler>();
            if (handler == null) handler = hitObject.GetComponentInChildren<HotspotHandler>();
            
            if (handler != null)
            {
                Debug.Log("✅ Κοιτάς: " + handler.hotspotName);
                
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("🖱️ ΚΛΙΚ! Ενεργοποίηση: " + handler.hotspotName);
                    handler.HandleClick();
                }
            }
        }
    }
}