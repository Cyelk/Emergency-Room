using UnityEngine;
using UnityEngine.EventSystems;

public class EHRPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerController playerController;
    private EHRManager ehrManager;
    
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        ehrManager = FindObjectOfType<EHRManager>();
    }
    
    // Όταν ο κέρσορας μπαίνει μέσα στο EHR Panel
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("🖱️ Κέρσορας μέσα στο EHR - Σταματάει η κάμερα");
        
        if (playerController != null)
        {
            playerController.canLook = false;
            playerController.canMove = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    // Όταν ο κέρσορας βγαίνει από το EHR Panel
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("🖱️ Κέρσορας έξω από το EHR");
        
        // Αν το EHR είναι ακόμα ανοιχτό, μην επαναφέρεις την κίνηση
        if (ehrManager != null && ehrManager.IsEHRPanelOpen())
        {
            // Το EHR είναι ανοιχτό - ο κέρσορας πρέπει να είναι ελεύθερος
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        
        // Το EHR είναι κλειστό - κλείδωσε τον κέρσορα
        if (playerController != null)
        {
            playerController.canLook = true;
            playerController.canMove = true;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}