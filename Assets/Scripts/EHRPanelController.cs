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
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerController != null)
        {
            playerController.canLook = false;
            playerController.canMove = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit - Selection: " + (ScenarioSelectionManager.Instance != null) + " | isPanelOpen: " + (ScenarioSelectionManager.Instance != null ? ScenarioSelectionManager.Instance.IsPanelOpen().ToString() : "N/A"));
        
        if (ScenarioSelectionManager.Instance != null && ScenarioSelectionManager.Instance.IsPanelOpen())
        {
            Debug.Log("Selection ανοιχτό - Αγνοώ");
            return;
        }
        
        if (HelpManager.Instance != null && HelpManager.Instance.IsHelpOpen())
        {
            Debug.Log("Help ανοιχτό - Αγνοώ");
            return;
        }
        
        if (ehrManager != null && ehrManager.IsEHRPanelOpen())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        
        if (playerController != null)
        {
            playerController.canLook = true;
            playerController.canMove = true;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}