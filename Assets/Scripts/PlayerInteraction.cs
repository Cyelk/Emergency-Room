using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 10f;
    
    private Camera playerCamera;
    private EHRManager ehrManager;
    
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        ehrManager = FindObjectOfType<EHRManager>();
    }
    
    void Update()
    {
        if (ehrManager != null && ehrManager.IsEHRPanelOpen())
            return;
        
        if (HelpManager.Instance != null && HelpManager.Instance.IsHelpOpen())
            return;
        
        if (ScenarioSelectionManager.Instance != null && ScenarioSelectionManager.Instance.IsPanelOpen())
            return;
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            HotspotHandler handler = hitObject.GetComponent<HotspotHandler>();
            if (handler == null) handler = hitObject.GetComponentInParent<HotspotHandler>();
            if (handler == null) handler = hitObject.GetComponentInChildren<HotspotHandler>();
            
            if (handler != null)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    handler.HandleClick();
                }
            }
        }
    }
}