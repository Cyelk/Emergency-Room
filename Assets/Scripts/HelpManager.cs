using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class HelpManager : MonoBehaviour
{
    public static HelpManager Instance;
    
    [Header("UI References")]
    public GameObject helpPanel;
    public ScrollRect scrollRect;
    
    [Header("Settings")]
    public bool resetScrollOnOpen = true; // true = κορυφή, false = θυμάται
    
    private bool isHelpOpen = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (helpPanel != null)
            helpPanel.SetActive(false);
        
        isHelpOpen = false;
    }
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
            ToggleHelp();
    }
    
    public void ToggleHelp()
    {
        if (isHelpOpen)
            CloseHelp();
        else
            OpenHelp();
    }
    
    public void OpenHelp()
    {
        if (helpPanel == null) return;
        
        helpPanel.SetActive(true);
        isHelpOpen = true;
        
        // Reset scroll στην κορυφή (αν το θέλεις)
        if (resetScrollOnOpen && scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("❓ Help άνοιξε");
    }
    
    public void CloseHelp()
    {
        if (helpPanel == null) return;
        
        helpPanel.SetActive(false);
        isHelpOpen = false;
        
        EHRManager ehr = FindObjectOfType<EHRManager>();
        if (ehr != null && ehr.IsEHRPanelOpen())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        Debug.Log("❓ Help έκλεισε");
    }
    
    public bool IsHelpOpen()
    {
        return isHelpOpen;
    }
}